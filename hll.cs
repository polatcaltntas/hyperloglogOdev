using System;
using System.Linq;

public class HyperLogLog {
    private readonly int p; // Precision bit sayısı
    private readonly int m; // Kova sayısı (2^p)
    private readonly byte[] registers;
    private readonly double alphaM;

    public HyperLogLog(int precision) {
        if (precision < 4 || precision > 16)
            throw new ArgumentException("Precision 4 ile 16 arasında olmalıdır.");

        p = precision;
        m = 1 << p; // 2^p
        registers = new byte[m];

        // Sabit düzeltme faktörü (alpha_m) hesaplama
        alphaM = CalculateAlphaM(m);
    }

    private double CalculateAlphaM(int m) {
        if (m == 16) return 0.673;
        if (m == 32) return 0.697;
        if (m == 64) return 0.709;
        return 0.7213 / (1 + 1.079 / m);
    }

    // Basit ve etkili bir 64-bit Hash fonksiyonu (MurmurHash3 benzeri)
    private ulong GetHash(string value) {
        using (var md5 = System.Security.Cryptography.MD5.Create()) {
            byte[] hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));
            return BitConverter.ToUInt64(hashBytes, 0);
        }
    }

    public void Add(string item) {
        ulong x = GetHash(item);
        int j = (int)(x >> (64 - p)); // İlk p bit kova indeksini belirler
        ulong w = x << p | (1UL << (p - 1)); // Geri kalan bitler

        // İlk 1 bitinin pozisyonunu bul (Leading Zeros + 1)
        byte rho = (byte)(Math.Min(LeadingZeros(w), 64 - p) + 1);

        // Kovadaki değeri güncelle (Sadece maksimum değeri tutarız)
        registers[j] = Math.Max(registers[j], rho);
    }

    private int LeadingZeros(ulong x) {
        if (x == 0) return 64;
        int n = 0;
        while ((x & 0x8000000000000000) == 0) {
            n++;
            x <<= 1;
        }
        return n;
    }

    public long Count() {
        // Harmonik Ortalama Hesaplama
        double sum = 0;
        for (int i = 0; i < m; i++) {
            sum += Math.Pow(2, -registers[i]);
        }

        double estimate = alphaM * m * m / sum;

        // Küçük veri seti düzeltmesi (Linear Counting)
        if (estimate <= 2.5 * m) {
            int v = registers.Count(r => r == 0);
            if (v != 0) {
                estimate = m * Math.Log((double)m / v);
            }
        }
        // Büyük veri seti düzeltmesi
        else if (estimate > (1.0 / 30.0) * Math.Pow(2, 32)) {
            estimate = -Math.Pow(2, 32) * Math.Log(1 - (estimate / Math.Pow(2, 32)));
        }

        return (long)estimate;
    }

    // Birleştirilebilirlik (Merge) Özelliği
    public void Merge(HyperLogLog other) {
        if (this.p != other.p) throw new Exception("Precision değerleri aynı olmalı!");
        for (int i = 0; i < m; i++) {
            this.registers[i] = Math.Max(this.registers[i], other.registers[i]);
        }
    }
}