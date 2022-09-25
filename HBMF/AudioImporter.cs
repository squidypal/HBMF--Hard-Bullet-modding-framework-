using MelonLoader;
using UnityEngine;
using FMODUnity;
using FMOD;
using System.Collections.Generic;

namespace AudioImporter
{
    public class AudioAPI
    {
        public static AudioClip Import(string location)
        {
            RuntimeManager.CoreSystem.createSound(MelonUtils.UserDataDirectory + "\\" + location, MODE._3D, out Sound sound);
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
        public bool Looping;
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
        public float Pitch
        {
            get
            {
                channel.getPitch(out float current);
                return current;
            }
            set
            {
                channel.setPitch(value);
            }
        }
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
        private readonly Dictionary<AudioInstance, bool> channels = new Dictionary<AudioInstance, bool>();

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
            foreach (KeyValuePair<AudioInstance, bool> channel in channels)
            {
                channel.Key.channel.set3DAttributes(ref pos, ref vel);
                if (channel.Value == false)
                {
                    channels[channel.Key] = true;
                    channel.Key.Paused = false;
                }
                channel.Key.channel.getCurrentSound(out Sound sound);
                sound.getLength(out uint length, TIMEUNIT.MS);
                if (channel.Key.Time == length)
                {
                    channels.Remove(channel.Key);
                }
                if (channel.Key.Looping == true && channel.Key.Time >= length - 100)
                {
                    channel.Key.Time = 0;
                }
            }
        }

        public AudioInstance Play(bool looping = false, float volume = 1f, float pitch = 1f, uint time = 0)
        {
            RuntimeManager.CoreSystem.playSound(clip.sound, new ChannelGroup(), true, out Channel channel);
            AudioInstance audioInstance = new AudioInstance(channel)
            {
                Looping = looping,
                Volume = volume,
                Pitch = pitch,
                Time = time
            };
            channels.Add(audioInstance, false);
            return audioInstance;
        }
        public void StopAll()
        {
            foreach (KeyValuePair<AudioInstance, bool> channel in channels)
            {
                channel.Key.Stop();
            }
        }
        public void SetPauseAll(bool paused)
        {
            foreach (KeyValuePair<AudioInstance, bool> channel in channels)
            {
                channel.Key.Paused = paused;
            }
        }
        public void SetVolumeAll(float volume)
        {
            foreach (KeyValuePair<AudioInstance, bool> channel in channels)
            {
                channel.Key.Volume = volume;
            }
        }
        public void SetPitchAll(float pitch)
        {
            foreach (KeyValuePair<AudioInstance, bool> channel in channels)
            {
                channel.Key.Pitch = pitch;
            }
        }
        public void SetTimeAll(uint time)
        {
            foreach (KeyValuePair<AudioInstance, bool> channel in channels)
            {
                channel.Key.Time = time;
            }
        }
    }
}
