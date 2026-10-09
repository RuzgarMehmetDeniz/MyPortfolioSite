# MyPortfolioSite

Kişisel portfolyo sitem. GitHub hesabımdaki projeleri otomatik çekip kart olarak gösterir. Yeni bir repo açtığımda siteye kendiliğinden eklenir.

🔗 **Canlı site:** [ruzgardeniz.runasp.net](https://ruzgardeniz.runasp.net/)

## Kullanılan Teknolojiler

- **ASP.NET Core MVC (.NET 8, C#):** Sitenin tamamı
- **Entity Framework Core + SQLite:** İletişim mesajlarını ve admin düzenlemelerini saklamak için
- **GitHub API:** Projeleri, README'leri ve görselleri çekmek için
- **Google Gemini API:** Proje asistanı (chatbot) için
- **Bootstrap 5, Bootstrap Icons, HTML, CSS, JavaScript:** Responsive (PC + mobil) arayüz

## Kullanıcı Arayüzü (UI)

Arayüz açık renkli, sade ve modern bir tasarıma sahip. Ana renk indigo/mor tonlarında, kartlar yuvarlatılmış köşeli ve hafif gölgeli. Tüm sayfalar bilgisayar, tablet ve telefonda düzgün görünecek şekilde responsive hazırlandı.

### Genel (tüm sayfalarda)

- **Hareketli arka plan:** Birbirine çizgilerle bağlanan noktalardan oluşan bir animasyon vardır. Fareyi hareket ettirince noktalar fareye doğru çekilir ve fareye çizgiyle bağlanır. Arkada yumuşak renkli bulanık ışık lekeleri (blob) bulunur.
- **Özel imleç:** Fareli cihazlarda küçük bir nokta ve onu yumuşakça takip eden bir halka imleç olarak görünür. Butonların ve kartların üstüne gelince halka büyür. Telefonda bu efekt kapalıdır.
- **Üst menü (navbar):** Sayfa kaydırılırken üstte sabit kalır. Ana Sayfa, Projeler ve İletişim bağlantıları ikonlarla gösterilir. Telefonda açılır menüye dönüşür.
- **Proje asistanı:** Sağ altta yuvarlak bir sohbet butonu bulunur. Tıklayınca küçük bir sohbet penceresi açılır. Kullanıcı mesajları mor, asistan cevapları gri baloncuk olarak görünür ve cevap beklenirken "Yazıyor…" yazar. Pencere telefon ekranına göre kendini boyutlandırır.
- **Alt bilgi (footer):** Tüm sayfaların altında yer alır.

### Ana sayfa

- **Hakkımda kartı:** "Yazılım Geliştirici" rozeti, renk geçişli isim başlığı, kısa tanıtım yazısı ve LinkedIn, GitHub, Instagram butonları (her biri kendi marka renginde ve ikonuyla).
- **Proje kartları:** Ekran genişliğine göre 3, 2 veya 1 sütunlu ızgara düzeninde sıralanır. Her kartta kapak görseli (görsel yoksa ikonlu yer tutucu), köşede programlama dili etiketi, proje başlığı, kısa açıklama ve "Detayları gör" bağlantısı vardır. Üzerine gelince kart hafifçe yükselir.
- **Sayfalama:** Sayfa başına 12 proje gösterilir. Önceki / Sonraki butonları ve numaralı sayfa düğmeleri vardır, çok sayfa olunca araya "…" girer.
- **İletişim bölümü:** Ad, e-posta ve mesaj alanlarından oluşan form. Gönderince başarılı veya hatalı olduğunu belirten renkli bir bilgi kutusu çıkar. Bot mesajlarını engellemek için gizli bir alan da vardır.

### Proje detay sayfası

- **Geri bağlantısı:** "Tüm projeler" ile ana sayfaya döner.
- **Başlık ve açıklama:** Projenin adı ve kısa tanıtımı.
- **Etiketler:** Programlama dili ve GitHub'daki konu (topic) etiketleri küçük rozetler halinde.
- **Görsel galerisi:** Yatay kaydırmalı şerit. Sağ ve sol oklarla yumuşak şekilde gezilir, bir görsele tıklayınca büyük hali yeni sekmede açılır. Görsel yoksa bu bölüm hiç görünmez.
- **Proje hakkında:** Projenin GitHub README'sinden alınan yazı ayrı bir kutuda gösterilir.
- **Butonlar:** "Projeyi göster" ve "GitHub".

### Projeyi göster sayfası

- **Canlı demo:** Projenin canlı adresi varsa sayfanın içinde açılır, yanında "Yeni sekmede aç" butonu bulunur.
- **Ekranlar:** Proje görselleri okla gezilen bir slayt (carousel) olarak gösterilir.
- **Boş durum:** Ne demo ne görsel varsa bilgi mesajı çıkar.

### Admin paneli

Şifreli giriş ile açılır. Panelden proje başlığı, açıklaması ve görselleri düzenlenebilir, projeler gizlenebilir veya öne çıkarılabilir. Siteden gelen iletişim mesajları da burada görülür.

## Ekran Görüntüleri

## <img width="1903" height="782" alt="image" src="https://github.com/user-attachments/assets/c15464fd-2df7-4eef-a7f5-4de143b6a1b8" />
## <img width="1897" height="738" alt="image" src="https://github.com/user-attachments/assets/ceca7959-7cf7-4343-9b7e-10c517aeb0e7" />
## <img width="1899" height="906" alt="image" src="https://github.com/user-attachments/assets/9c7bbc68-b511-4290-a5c4-ecf249558607" />
## <img width="1896" height="909" alt="image" src="https://github.com/user-attachments/assets/880b6a3c-9d0c-4503-9365-9991a3a6674e" />
## <img width="1817" height="906" alt="image" src="https://github.com/user-attachments/assets/4659c18a-710d-43d0-b4d8-94c763fabbdb" />
## <img width="1807" height="748" alt="image" src="https://github.com/user-attachments/assets/995d0ff6-695c-4b0a-983e-f3b00890b6b3" />
## <img width="1899" height="911" alt="image" src="https://github.com/user-attachments/assets/26115908-2572-4ede-99b4-e3b4e2326f70" />
