using FMOD;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace AudioImporter
{
    public class AudioAPI
    {
        public static AudioClip Import(string location)
        {
            RuntimeManager.CoreSystem.createSound(location, MODE.LOOP_NORMAL, out Sound sound);
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
        public Channel channel;
        public bool Use2D
        {
            get
            {
                channel.getMode(out MODE current);
                return current == (MODE.LOOP_NORMAL | MODE._2D);
            }
            set
            {
                if (value == true)
                {
                    channel.setMode(MODE.LOOP_NORMAL | MODE._2D);
                }
                else
                {
                    channel.setMode(MODE.LOOP_NORMAL | MODE._3D);
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

        public AudioInstance(Channel newChannel)
        {
            channel = newChannel;
        }
    }

    public class AudioSource : MonoBehaviour
    {
        public AudioClip clip;
        private VECTOR pos = new VECTOR();
        private VECTOR vel = new VECTOR();
        private VECTOR lastPos = new VECTOR();
        private readonly Dictionary<AudioInstance, bool> setUpInstances = new Dictionary<AudioInstance, bool>();

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
            List<AudioInstance> unpause = new List<AudioInstance>();
            List<AudioInstance> remove = new List<AudioInstance>();
            foreach (KeyValuePair<AudioInstance, bool> instance in setUpInstances)
            {
                if (instance.Key.Attached == true || instance.Value == false)
                {
                    instance.Key.channel.set3DAttributes(ref pos, ref vel);
                }
                if (instance.Key.UseSlowMotion == true)
                {
                    instance.Key.channel.setPitch(instance.Key.Pitch * Time.timeScale);
                }
                else
                {
                    instance.Key.channel.setPitch(instance.Key.Pitch);
                }
                if (instance.Value == false)
                {
                    unpause.Add(instance.Key);
                }
                instance.Key.channel.getCurrentSound(out Sound sound);
                sound.getLength(out uint length, TIMEUNIT.MS);
                if (instance.Key.Time == length)
                {
                    remove.Add(instance.Key);
                }
                if (instance.Key.Looping == true && instance.Key.Time >= length - 100)
                {
                    instance.Key.Time = 0;
                }
            }
            foreach (AudioInstance instance in unpause)
            {
                setUpInstances[instance] = true;
                instance.Paused = false;
            }
            foreach (AudioInstance instance in remove)
            {
                setUpInstances.Remove(instance);
            }
        }

        public AudioInstance Play(PlaySettings playSettings = null)
        {
            if (playSettings == null)
            {
                playSettings = new PlaySettings();
            }
            RuntimeManager.CoreSystem.playSound(clip.sound, new ChannelGroup(), true, out Channel channel);
            AudioInstance audioInstance = new AudioInstance(channel)
            {
                Use2D = playSettings.Use2D,
                Looping = playSettings.Looping,
                Volume = playSettings.Volume,
                Pitch = playSettings.Pitch,
                UseSlowMotion = playSettings.UseSlowMotion,
                Attached = playSettings.Attached,
                Time = playSettings.Time
            };
            setUpInstances.Add(audioInstance, false);
            return audioInstance;
        }
        public void StopAll()
        {
            foreach (KeyValuePair<AudioInstance, bool> instance in setUpInstances)
            {
                instance.Key.Stop();
            }
        }
        public void SetUse2DAll(bool use2D)
        {
            foreach (KeyValuePair<AudioInstance, bool> setUpInstance in setUpInstances)
            {
                setUpInstance.Key.Use2D = use2D;
            }
        }
        public void SetLoopingAll(bool looping)
        {
            foreach (KeyValuePair<AudioInstance, bool> setUpInstance in setUpInstances)
            {
                setUpInstance.Key.Looping = looping;
            }
        }
        public void SetVolumeAll(float volume)
        {
            foreach (KeyValuePair<AudioInstance, bool> setUpInstance in setUpInstances)
            {
                setUpInstance.Key.Volume = volume;
            }
        }
        public void SetPitchAll(float pitch)
        {
            foreach (KeyValuePair<AudioInstance, bool> setUpInstance in setUpInstances)
            {
                setUpInstance.Key.Pitch = pitch;
            }
        }
        public void SetUseSlowMotionAll(bool useSlowMotion)
        {
            foreach (KeyValuePair<AudioInstance, bool> setUpInstance in setUpInstances)
            {
                setUpInstance.Key.UseSlowMotion = useSlowMotion;
            }
        }
        public void SetAttachedAll(bool attached)
        {
            foreach (KeyValuePair<AudioInstance, bool> setUpInstance in setUpInstances)
            {
                setUpInstance.Key.Attached = attached;
            }
        }
        public void SetTimeAll(uint time)
        {
            foreach (KeyValuePair<AudioInstance, bool> setUpInstance in setUpInstances)
            {
                setUpInstance.Key.Time = time;
            }
        }
        public void SetPauseAll(bool paused)
        {
            foreach (KeyValuePair<AudioInstance, bool> setUpInstance in setUpInstances)
            {
                setUpInstance.Key.Paused = paused;
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
