# HyperLogLog in C#

Bu proje, **HyperLogLog algoritmasının C# ile temel bir implementasyonunu** içerir. Amaç, büyük veri kümelerinde **benzersiz (unique) eleman sayısını yaklaşık olarak** hesaplamaktır.

## HyperLogLog Nedir?

HyperLogLog, bir veri kümesindeki farklı eleman sayısını tam olarak değil, **yaklaşık olarak** bulan olasılıksal bir algoritmadır. Büyük veri kümelerinde çok az bellek kullanarak hızlı sonuç üretmesi en büyük avantajıdır.

## Çalışma Mantığı

Algoritma şu adımlarla çalışır:

1. Her eleman hash’lenir.
2. Hash’in ilk bitleri ile bir **register (kova)** seçilir.
3. Kalan bitlerdeki **baştaki sıfır sayısı** hesaplanır.
4. Her register yalnızca gördüğü **en büyük değeri** saklar.
5. Tüm register’lar kullanılarak yaklaşık benzersiz eleman sayısı hesaplanır.

## Özellikler

- Yaklaşık unique eleman sayımı
- Düşük bellek kullanımı
- Küçük veri kümeleri için düzeltme
- Büyük veri kümeleri için düzeltme
- İki HyperLogLog nesnesini birleştirme (`Merge`) desteği

## Kullanım

### Nesne oluşturma

`var hll = new HyperLogLog(10);`

### Veri ekleme

`hll.Add("Ali");`
`hll.Add("Veli");`
`hll.Add("Ali");`
`hll.Add("Ayşe");`

### Tahmini sonucu alma

`long count = hll.Count();`
`Console.WriteLine($"Tahmini benzersiz eleman sayısı: {count}");`

## Merge Örneği

`var hll1 = new HyperLogLog(10);`
`hll1.Add("Ali");`
`hll1.Add("Veli");`

`var hll2 = new HyperLogLog(10);`
`hll2.Add("Ayşe");`
`hll2.Add("Veli");`

`hll1.Merge(hll2);`

`Console.WriteLine($"Birleşik tahmini sayı: {hll1.Count()}");`

## Metotlar

- `HyperLogLog(int precision)` → sınıfı başlatır
- `Add(string item)` → yeni eleman ekler
- `Count()` → yaklaşık benzersiz eleman sayısını döndürür
- `Merge(HyperLogLog other)` → iki HyperLogLog yapısını birleştirir

## Notlar

- Bu implementasyonda hash işlemi için **MD5** kullanılmıştır.
- Kod, algoritmanın mantığını göstermek amacıyla hazırlanmıştır.
- Sonuçlar tam değil, yaklaşık sonuçlardır.

## Kullanım Alanları

- Web analitiği
- Log analizi
- Büyük veri işleme
- Dağıtık sistemler
- Unique kullanıcı / IP / ID sayımı

## Sonuç

Bu proje, HyperLogLog algoritmasının C# dilinde temel çalışma mantığını göstermektedir. Özellikle büyük veri kümelerinde benzersiz eleman sayısını düşük bellek kullanarak tahmin etmek için kullanılır.
