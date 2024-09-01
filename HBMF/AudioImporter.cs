using HBMF.AudioImporter.Internal;
using Il2CppFMOD;
using Il2CppFMODUnity;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HBMF.AudioImporter
{
    public class Audio
    {
        public static AudioClip Import(byte[] bytes)
        {
            AudioClip clip = new();
            MelonCoroutines.Start(WaitAndImport(bytes, clip));
            return clip;
        }

        public static AudioClip Import(string file)
        {
            AudioClip clip = new();
            MelonCoroutines.Start(WaitAndImport(file, clip));
            return clip;
        }

        private static IEnumerator WaitAndImport(byte[] bytes, AudioClip clip)
        {
            yield return new WaitForCoreSystem();
            string file = Path.GetTempFileName();
            File.WriteAllBytes(file, bytes);
            RuntimeManager.CoreSystem.createSound(file, MODE.LOOP_NORMAL | MODE._3D, out Sound sound);
            clip.sound = sound;
        }

        private static IEnumerator WaitAndImport(string file, AudioClip clip)
        {
            yield return new WaitForCoreSystem();
            RuntimeManager.CoreSystem.createSound(file, MODE.LOOP_NORMAL | MODE._3D, out Sound sound);
            clip.sound = sound;
        }
    }

    public class AudioClip
    {
        internal Sound sound;
    }

    public class AudioInstance
    {
        internal Channel channel;
        public bool Use2D
        {
            get
            {
                channel.getMode(out MODE current);
                return current == (MODE.LOOP_NORMAL | MODE.OPENMEMORY | MODE._2D);
            }
            set
            {
                channel.setMode(MODE.LOOP_NORMAL | MODE.OPENMEMORY | (value ? MODE._2D : MODE._3D));
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
                channel.setLoopCount(value ? -1 : 0);
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
    }

    [RegisterTypeInIl2Cpp]
    public class AudioSource : MonoBehaviour
    {
        public AudioClip clip;
        private VECTOR pos = new();
        private VECTOR vel = new();
        private VECTOR lastPos = new();
        private readonly List<AudioInstance> instances = new();

        public AudioSource(IntPtr ptr) : base(ptr) { }

        internal void Update()
        {
            pos = new()
            {
                x = transform.position.x,
                y = transform.position.y,
                z = transform.position.z
            };
            vel = new()
            {
                x = pos.x - lastPos.x,
                y = pos.y - lastPos.y,
                z = pos.z - lastPos.z
            };
            lastPos = pos;
            foreach (AudioInstance instance in instances.ToArray())
            {
                if (instance.Attached)
                {
                    instance.channel.set3DAttributes(ref pos, ref vel);
                }
                if (instance.UseSlowMotion)
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
                    instances.Remove(instance);
                }
            }
        }

        [HideFromIl2Cpp]
        public AudioInstance Play(PlaySettings playSettings = null)
        {
            playSettings ??= new();
            RuntimeManager.CoreSystem.playSound(clip.sound, new(), true, out Channel newChannel);
            AudioInstance audioInstance = new()
            {
                channel = newChannel,
                Use2D = playSettings.Use2D,
                Looping = playSettings.Looping,
                Volume = playSettings.Volume,
                Pitch = playSettings.Pitch,
                UseSlowMotion = playSettings.UseSlowMotion,
                Attached = playSettings.Attached,
                Time = playSettings.Time,
                Paused = false,
            };
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

        internal void OnDestroy()
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

namespace HBMF.AudioImporter.Internal
{
    [RegisterTypeInIl2Cpp]
    public class WaitForCoreSystem : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                return !RuntimeManager.IsInitialized;
            }
        }

        public WaitForCoreSystem(IntPtr ptr) : base(ptr) { }

        public WaitForCoreSystem() : base(ClassInjector.DerivedConstructorPointer<WaitForCoreSystem>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }
    }
}
