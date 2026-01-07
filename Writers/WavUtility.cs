using System;
using System.IO;
using UnityEngine;
using Helpers;

public static class WavUtility
{
    public static void SaveWav(string filename, AudioClip clip)
    {
        var filepath = Path.ChangeExtension(filename, ".wav");
        Helpers.SaveSystemMethods.CheckOrCreateDirectory(Path.GetDirectoryName(filepath));

        using (var file = new FileStream(filepath, FileMode.Create))
        using (var writer = new BinaryWriter(file))
        {
            int sampleCount = clip.samples * clip.channels;
            float[] samples = new float[sampleCount];
            clip.GetData(samples, 0);

            ushort bitDepth = 16;
            int byteCount = sampleCount * (bitDepth / 8);

            // WAV HEADER
            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(36 + byteCount);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((ushort)1);
            writer.Write((ushort)clip.channels);
            writer.Write(clip.frequency);
            writer.Write(clip.frequency * clip.channels * (bitDepth / 8));
            writer.Write((ushort)(clip.channels * (bitDepth / 8)));
            writer.Write(bitDepth);

            // DATA
            writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            writer.Write(byteCount);

            // Sample data
            foreach (var sample in samples)
            {
                short val = (short)(sample * short.MaxValue);
                writer.Write(val);
            }
        }

        Debug.Log($"Saved WAV to: {filepath}");
    }

    public static AudioClip TrimClip(AudioClip clip, int samplesToKeep) {
        float[] data = new float[samplesToKeep];
        clip.GetData(data, 0);

        AudioClip trimmed = AudioClip.Create(
            "TrimmedClip",
            samplesToKeep,
            clip.channels,
            clip.frequency,
            false
        );

        trimmed.SetData(data, 0);
        return trimmed;
    }
}