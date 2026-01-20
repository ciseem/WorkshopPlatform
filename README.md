# Workshop Platform: Kurumsal Eğitim ve Atölye Yönetim Ekosistemi

Workshop Platform, profesyonel eğitim süreçlerinin dijital ortamda uçtan uca yönetilmesi amacıyla geliştirilmiş, yüksek performanslı ve ölçeklenebilir bir **Eğitim Yönetim Sistemi (LMS)** çözümüdür. Modern yazılım mimarisi prensipleriyle (Clean Architecture) tasarlanan platform, hem eğitmenler hem de katılımcılar için optimize edilmiş bir kullanıcı deneyimi sunar.

---

## 🏗️ Teknik Mimari ve Tasarım Prensipleri

Platform, yönetilebilirlik ve sürdürülebilirlik odaklı **Katmanlı Mimari (Layered Architecture)** üzerine inşa edilmiştir.

### 📚 Katman Detayları
1.  **WorkshopPlatform.Core**: Sistemdeki temel iş kurallarını (Business Rules), veri modellerini (Entities), DTO yapılarını ve servis arayüzlerini (Interfaces) barındıran merkezi katman. Herhangi bir dış bağımlılıktan izoledir.
2.  **WorkshopPlatform.Data**: Veri erişim katmanı. **Entity Framework Core** kullanılarak veritabanı etkileşimlerini yönetir. Repository desenine uygun servis uygulamalarını ve veritabanı bağlamını (DbContext) içerir.
3.  **WorkshopPlatform.Web**: Kullanıcı ile etkileşimin sağlandığı **ASP.NET Core 9.0 MVC** katmanı. Modern arayüz bileşenleri, Controller yapıları ve sunum mantığı burada yer alır.

### 🎨 Tasarım Sistemi (UI/UX)
Sistem, herhangi bir ağır CSS kütüphanesine (Bootstrap/Tailwind) bağımlı kalmadan, tamamen özel geliştirilmiş bir **Vanilla CSS** framework'ü kullanır:
- **Tipografi**: Okunabilirlik oranı yüksek **Inter** ve **Outfit** font aileleri.
- **Efektler**: Modern **Glassmorphism** (cam efekti) ve yumuşak geçişli **Mesh Gradient** arka planlar.
- **Arayüz**: Tamamen responsive (mobil uyumlu) ve kurumsal kimliğe uygun "Dark/Light" dengeli tasarım sistemi.

---

## 🛠️ Teknoloji Yığını

| Bileşen | Teknoloji | Açıklama |
| :--- | :--- | :--- |
| **Framework** | `.NET 9.0` | En güncel C# ve ASP.NET Core altyapısı. |
| **Veritabanı** | `MSSQL Server` | Kurumsal düzeyde veri saklama ve ilişkilendirme. |
| **ORM** | `EF Core` | Database-First yaklaşımı ile optimize edilmiş veri erişimi. |
| **Raporlama** | `QuestPDF` | Kod tabanlı, yüksek kaliteli PDF belge üretimi. |
| **Frontend** | `Vanilla CSS & JS` | Performans odaklı, kütüphane bağımsız UI yönetimi. |
| **Harita** | `Leaflet.js` | Open-source interaktif harita entegrasyonu. |

---

## 🚀 Fonksiyonel Modüller

### 1. Eğitim Kategorileri ve Keşif
Sistem, eğitimleri uzmanlık alanlarına göre kategorize ederek katılımcıların doğru içeriğe hızlıca ulaşmasını sağlar.

<div align="center">
    <img src="/WorkshopPlatform.Web/wwwroot/images/software.png" width="100" />
    <img src="/WorkshopPlatform.Web/wwwroot/images/design.png" width="100" />
    <img src="/WorkshopPlatform.Web/wwwroot/images/business.png" width="100" />
    <img src="/WorkshopPlatform.Web/wwwroot/images/marketing.png" width="100" />
    <p><i>Yazılım, Tasarım, İş Dünyası ve Pazarlama odaklı zengin içerik kataloğu.</i></p>
</div>

### 2. Eğitmen Yönetim Paneli
Eğitmenler, kendi dikeyindeki tüm süreçleri tek bir kontrol panelinden yönetebilir:
- **Workshop Planlama**: Eğitim başlığı, detaylı açıklama, tarih ve kontenjan yönetimi.
- **Lokasyon Belirleme**: Eğitim mekanının harita üzerinde koordinat bazlı işaretlenmesi.
- **Katılımcı Takibi**: Kayıtlı kullanıcıların listelenmesi, katılım durumlarının onaylanması.
- **Finansal Analiz**: Eğitim başına elde edilen gelirin ve genel finansal durumun takibi.

### 3. Katılımcı ve Kayıt Sistemi
Kullanıcılar için sadeleştirilmiş ve güvenli bir başvuru süreci:
- **Kişisel Dashboard**: Takip edilen eğitimler ve geçmiş workshop kayıtlarının yönetimi.
- **Anlık Kayıt**: Kontenjan kontrolü ile hızlı katılım sağlama.
- **Güvenli Kimlik Doğrulama**: Şifre sıfırlama süreçlerinde e-posta/kod bazlı doğrulama mekanizması.

---

## 📊 Raporlama ve Analitik
Sistem, kurumsal raporlama ihtiyaçları için gelişmiş çıktı seçenekleri sunar:
- **Katılımcı Listesi (PDF/CSV)**: Eğitim öncesi hazırlıklar için detaylı katılımcı dökümleri.
- **Finansal Özetler**: Dönemsel gelir raporları ve verimlilik analizleri.

---

## ⚙️ Kurulum ve Yapılandırma

### Veritabanı Kurulumu
1. SQL Server Management Studio (SSMS) uygulamasını başlatın.
2. Proje ana dizininde bulunan `database.sql` script dosyasını açın.
3. `Execute` (F5) komutu ile tabloları ve hazır verileri oluşturun.

### Uygulama Ayarları
`WorkshopPlatform.Web/appsettings.json` dosyasındaki bağlantı dizesini düzenleyin:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=WorkshopPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Derleme ve Çalıştırma
```powershell
# Proje dizinine gidin
cd WorkshopPlatform.Web

# Bağımlılıkları yükleyin ve projeyi derleyin
dotnet build

# Uygulamayı başlatın
dotnet run
```

---

## 🔐 Test Hesapları

| Kullanıcı Rolü | E-posta Adresi | Şifre |
| :--- | :--- | :--- |
| **Yönetici/Eğitmen** | `instructor@test.com` | `123456` |
| **Katılımcı** | `student@test.com` | `123456` |

---

 🎥 **YouTube:**https://youtu.be/lEbRWbjsKMI





<div align="center">

**Workshop Platform v5.0**  
*Kurumsal Eğitim Yönetiminde Teknik Mükemmeliyet*

</div>
