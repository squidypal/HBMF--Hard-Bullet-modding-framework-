using FMOD;
using FMODUnity;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AudioImporter
{
    public class AudioAPI
    {
        public static AudioClip Import(Assembly assembly, string location)
        {
            byte[] bytes = EmbeddedAssetBundle.LoadFromAssembly(assembly, location);
            CREATESOUNDEXINFO exInfo = new CREATESOUNDEXINFO()
            {
                cbsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CREATESOUNDEXINFO)),
                length = (uint)bytes.Length
            };
            RuntimeManager.CoreSystem.createSound(bytes, MODE.LOOP_NORMAL | MODE.OPENMEMORY | MODE._3D, ref exInfo, out Sound sound);
            return new AudioClip(sound);
        }
        public static AudioSource CreateSource(GameObject gameObject, AudioClip clip)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            return audioSource;
        }
    }

    public class AudioClip
    {
        public Sound sound;

        public AudioClip(Sound newSound)
        {
            sound = newSound;
        }
    }

    public class AudioInstance
    {
        public readonly Channel channel;
        public bool Use2D
        {
            get
            {
                channel.getMode(out MODE current);
                return current == (MODE.LOOP_NORMAL | MODE.OPENMEMORY | MODE._2D);
            }
            set
            {
                if (value == true)
                {
                    channel.setMode(MODE.LOOP_NORMAL | MODE.OPENMEMORY | MODE._2D);
                }
                else
                {
                    channel.setMode(MODE.LOOP_NORMAL | MODE.OPENMEMORY | MODE._3D);
                }
            }
        }
        public bool Looping
        {
            get
            {
                channel.getLoopCount(out int current);
                return current == -1;
            }
            set
            {
                if (value == true)
                {
                    channel.setLoopCount(-1);
                }
                else
                {
                    channel.setLoopCount(0);
                }
            }
        }
        public float Volume
        {
            get
            {
                channel.getVolume(out float current);
                return current;
            }
            set
            {
                channel.setVolume(value);
            }
        }
        public float Pitch;
        public bool UseSlowMotion;
        public bool Attached;
        public uint Time
        {
            get
            {
                channel.getPosition(out uint current, TIMEUNIT.MS);
                return current;
            }
            set
            {
                channel.setPosition(value, TIMEUNIT.MS);
            }
        }
        public bool Paused
        {
            get
            {
                channel.getPaused(out bool current);
                return current;
            }
            set
            {
                channel.setPaused(value);
            }
        }

        public void Stop()
        {
            channel.stop();
        }

        public AudioInstance(Channel newChannel, PlaySettings playSettings)
        {
            channel = newChannel;
            Use2D = playSettings.Use2D;
            Looping = playSettings.Looping;
            Volume = playSettings.Volume;
            Pitch = playSettings.Pitch;
            UseSlowMotion = playSettings.UseSlowMotion;
            Attached = playSettings.Attached;
            Time = playSettings.Time;
            Paused = false;
        }
    }

    public class AudioSource : MonoBehaviour
    {
        public AudioClip clip;
        private VECTOR pos = new VECTOR();
        private VECTOR vel = new VECTOR();
        private VECTOR lastPos = new VECTOR();
        private readonly List<AudioInstance> instances = new List<AudioInstance>();

        private void Update()
        {
            pos = new VECTOR
            {
                x = transform.position.x,
                y = transform.position.y,
                z = transform.position.z
            };
            vel = new VECTOR
            {
                x = pos.x - lastPos.x,
                y = pos.y - lastPos.y,
                z = pos.z - lastPos.z
            };
            lastPos = pos;
            List<AudioInstance> remove = new List<AudioInstance>();
            foreach (AudioInstance instance in instances)
            {
                if (instance.Attached == true)
                {
                    instance.channel.set3DAttributes(ref pos, ref vel);
                }
                if (instance.UseSlowMotion == true)
                {
                    instance.channel.setPitch(instance.Pitch * Time.timeScale);
                }
                else
                {
                    instance.channel.setPitch(instance.Pitch);
                }
                instance.channel.getCurrentSound(out Sound sound);
                sound.getLength(out uint length, TIMEUNIT.MS);
                if (length == 0)
                {
                    remove.Add(instance);
                }
            }
            foreach (AudioInstance instance in remove)
            {
                instances.Remove(instance);
            }
        }

        public AudioInstance Play(PlaySettings playSettings = null)
        {
            if (playSettings == null)
            {
                playSettings = new PlaySettings();
            }
            RuntimeManager.CoreSystem.playSound(clip.sound, new ChannelGroup(), true, out Channel channel);
            AudioInstance audioInstance = new AudioInstance(channel, playSettings);
            instances.Add(audioInstance);
            return audioInstance;
        }
        public void StopAll()
        {
            foreach (AudioInstance instance in instances)
            {
                instance.Stop();
            }
        }
        public void SetUse2DAll(bool use2D)
        {
            foreach (AudioInstance instance in instances)
            {
                instance.Use2D = use2D;
            }
        }
        public void SetLoopingAll(bool looping)
        {
            foreach (AudioInstance instance in instances)
            {
                instance.Looping = looping;
            }
        }
        public void SetVolumeAll(float volume)
        {
            foreach (AudioInstance instance in instances)
            {
                instance.Volume = volume;
            }
        }
        public void SetPitchAll(float pitch)
        {
            foreach (AudioInstance instance in instances)
            {
                instance.Pitch = pitch;
            }
        }
        public void SetUseSlowMotionAll(bool useSlowMotion)
        {
            foreach (AudioInstance instance in instances)
            {
                instance.UseSlowMotion = useSlowMotion;
            }
        }
        public void SetAttachedAll(bool attached)
        {
            foreach (AudioInstance instance in instances)
            {
                instance.Attached = attached;
            }
        }
        public void SetTimeAll(uint time)
        {
            foreach (AudioInstance instance in instances)
            {
                instance.Time = time;
            }
        }
        public void SetPauseAll(bool paused)
        {
            foreach (AudioInstance instance in instances)
            {
                instance.Paused = paused;
            }
        }

        private void OnDestroy()
        {
            StopAll();
        }
    }

    public class PlaySettings
    {
        public bool Use2D = false;
        public bool Looping = false;
        public float Volume = 1f;
        public float Pitch = 1f;
        public bool UseSlowMotion = true;
        public bool Attached = true;
        public uint Time = 0;
    }
}