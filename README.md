# Yapay Zeka Destekli TPS-Third Person Shooter Oyunu

### Kocaeli Üniversitesi Teknoloji Fakültesi  
**Bilişim Sistemleri Mühendisliği – Yazılım Geliştirme Laboratuvarı I (2025–2026 Güz Dönemi)**

---

##  Takım Üyeleri

| Ad Soyad | Öğrenci No | Sorumluluklar |
|-----------|-------------|----------------|
| **İrem Kalaycı** | 231307047 | Yapay zekâ sistemleri, NPC davranışları, ses & müzik,sahne |
| **Muhammed Taha Kızıkoğlu** | 241307121 | NPC davranışları,UI/UX, menü sistemleri, sahne tasarımı|
| **Turkay Jafarli** | 221307112 | Player mekanikleri,Player davranışları, ışıklandırma,optimizasyon.|

---

##  Proje Özeti

Bu proje, **Ay yüzeyinde geçen üçüncü şahıs nişancı (TPS)** türünde bir bilim kurgu aksiyon oyunudur.  
Oyuncu , *Helios* Uzay Üssü’nü istilacı yaratıklardan korumakla görevlidir.  

Oyun;  
- **FSM (Finite State Machine)** tabanlı **NPC yapay zekâ sistemi**,  
- **NavMesh Agent** ile yol bulma ve hedef takibi,  
- **Gerçek zamanlı sağlık, mermi, ses ve ışık yönetimi**,  
- **UI ve Menü sistemleri**  
ile desteklenmiştir.

---

## Senaryo

> **Yıl 2097.**  
> Ay’daki *Helios* araştırma üssü, bir enerji deneyi sonrası saatlerin bozulması sonucu zaman karmaşası yaşar bu karmaşa esnasında mutant istilasına uğrar.  
> Oyuncu, üsdeki son güvenlik personeli olarak bölgeyi korumak ve iletişim antenlerini yeniden aktive etmek zorundadır.

### Oyun Dünyası
- Kraterli **ay yüzeyi (Terrain)**  
- **Bilim kurgu ofis alanları (Sci-Fi ofis)**  
- **Uçan UFO trafiği** (animasyonlu loop sistem)  
- **Yumuşatılmış ışık sistemi** (Directional kaldırıldı, Point/Area Light kullanıldı)

---

## Oyun Mekanikleri

### Oyuncu
- **Hareket:** W, A, S, D  
- **Koşma:** Shift  
- **Zıplama:** Space  
- **Nişan Alma:** Sağ Tık  
- **Ateş Etme:** Sol Tık  
- **Şarjör Değiştirme:** R  
- **Pause / Menü:** Esc  

**Ses efektleri:** silah sesi, adım sesi, jump-attack, ölüm efekti  
**Sağlık Sistemi:**  
- `PlayerHealth` scripti ile damage hesaplanır.  
- *Pumpkin Heal* sistemiyle balkabaklarına dokunulduğunda sağlık artar.  

 **Mermi Sistemi:**  
- UI ile senkronize **ammo counter** ve reload animasyonu.  
- **Cephane bittiğinde** oyuncu otomatik olarak uyarı alır.  

---

##  NPC Yapay Zekâ (FSM + NavMesh)

### FSM Durumları
- **Idle:** Boşta bekleme  
- **Patrol:** Belirlenen noktalar arasında devriye  
- **Chase:** Oyuncuyu fark edip kovalamaya başlama  
- **Attack:** Yakın veya uzak saldırı  
- **Death:** Ölüm animasyonu, collider devre dışı, destroy  

### NPC Türleri

#### Zombie (Yakın Dövüş)
- Yavaş hareket eder, oyuncuya yaklaşınca saldırır.  
- FSM: Idle → Patrol → Chase → Attack → Death  
- Saldırı animasyonuna hasar tetikleyici event eklenmiştir.

#### Ely (Jump Attack NPC)
- Mixamo karakter “Ely By K.Atienza”.  
- Zıplayarak saldırır, yere düştüğünde kısa stun.  
- FSM: Walking → Running → Jump Attack → FallingBackDeath  

#### Asker (Ranged)
- Uzaktan ateş eder, mermi prefab’ı `EnemyBullet`.  
- FSM: Idle → Patrol → Chase → Aim → Shoot → Reload  

---

## Teknik Özellikler ve Kullanılan Teknolojiler

| Kategori | Teknoloji / Sistem |
|-----------|--------------------|
| Oyun Motoru | **Unity 6000.2.7f2 (LTS)** |
| Dil | **C#** |
| Yapay Zekâ | **FSM (Finite State Machine)** + **NavMesh Agent** |
| Kamera | **Cinemachine (3rd-person follow)** |
| Arayüz | **TextMeshPro, Canvas UI, Health & Ammo Bars** |
| Görsel Efekt | **Post-Processing (Bloom, Vignette, Lens Distortion)** |
| Ses Sistemi | **AudioMixer (Master / Music / SFX)** |
| Optimizasyon | **LOD, Static Batching, Object Pooling** |
| Işıklandırma | **Point + Area Light kombinasyonu**, Ambient artırıldı |
| Sürüm Kontrol | **Git / GitHub + Git LFS (FBX, PNG, TGA)** |

---

## Ek Sistemler

### Pumpkin Heal
- “Healing Pumpkin” objesiyle etkileşime geçildiğinde player HP yenilenir.  
- Trigger → `OnTriggerEnter` → `PlayerHealth.Heal(amount)`  
- Konsolda: `"Healing Pumpkin: +x HP"` çıktısı görülür.  

### UFO Sistemi
- UFO objeleri **looped animation** veya **spline-based path** ile uçuşta.  
- `Animator` + `Transform.RotateAround()` kombinasyonu.   

### Sci-Fi Ofisler
- Low-poly asset’ler, **emissive materyal** destekli.  

---

## Test Edilen Özellikler

| Test Alanı | Sonuç |
|-------------|--------|
| FSM geçişleri | Stabil |
| NavMesh pathfinding | Engel tespiti başarılı |
| Player hasar & heal sistemi | Çalışıyor |
| Ranged saldırılar | Sphere trigger & hasar senkronize |
| FPS performansı | 60+ FPS |
| Işıklandırma | Gerçekçi, kararma sorunu çözülmüş |

---

## Karşılaşılan Sorunlar ve Çözümler

| Sorun | Çözüm |
|-------|--------|
| Player arkası kararıyor | Directional Light kaldırılıp ambient ve fill light artırıldı |
| Mermiler player’dan geçiyor | Collider `isTrigger=true`, Rigidbody `ContinuousDynamic` |
| NPC navmesh sapması | `stoppingDistance` ve `autoBraking` optimize edildi |
| Jump-Attack iniş bug’ı | `updatePosition=false` ile root motion düzeltildi |
| Karakter silahı tutmuyordu |	IK ile el pozisyonu ayarlanarak düzeltildi |
| Karakter kayması |	Animator root motion devre dışı bırakılıp hareket kodu Rigidbody/CharacterController üzerinden kontrol edildi |
| Zoom sonrası bacakların sabit kalması |	Zoom sırasında Animator layer weight geçişleri optimize edildi |
| Crosshair hizalanmıyordu |	FirePoint yönü kamera forward vektörüyle senkronize edildi |
| Sağ tıkla aim sonrası kamera-silah hizası bozuk |	PlayerAimAlign scriptiyle kamera ve gövde rotasyonu senkronize edildi |
| GameOver yazısı erken çıkıyordu |	Can değeri kontrolü <= 0 yerine == 0 sonrası tetiklenecek şekilde güncellendi |
| FirePoint nişan hattı yamuk |	Silah rig’inde FirePoint yeniden hizalandı; uç nokta raycast’i kamera merkez noktasına bakacak şekilde ayarlandı |
| Idle → Walk/Run tetiklenmiyor |	Animator parametre adları/koşulları (Speed/IsMoving) düzeltildi; “Write Defaults” sahneye uygun hale getirildi |
| Karakter yerden düşüyor (spawn’da) | İlk karede yer kontrolünü bekle (yield/Invoke), doğru zemin layer’ıyla groundCheck yap |
| Materyaller pembe (URP/HDRP) | Render Pipeline Asset atandı mı? Materyalleri “Upgrade to URP/HDRP Materials” ile dönüştür |
| Ses çok yüksek/alçak | AudioMixer kullan, SFX grubunda dB seviyelerini normalize et; 3D rolloff eğrisi ayarla |
---

## Kazanımlar
- FSM yapısının gerçek zamanlı kontrolü  
- NavMeshAgent tabanlı hedef takibi  
- Çoklu NPC tipiyle etkileşim sistemi  
- AudioMixer optimizasyonu  
- Terrain ve aydınlatma senkronizasyonu  
- Git LFS ile asset yönetimi  

---

## Kaynakça
- Unity AI Navigation Docs : https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/index.html
- Mixamo Character & Animation Library: mixamo.com
- *Game Programming Patterns* – Robert Nystrom : https://gameprogrammingpatterns.com/
- Unity Post-Processing Stack : https://docs.unity3d.com/Packages/com.unity.postprocessing@3.5/manual/index.html
- Unity AudioMixer Manual : https://docs.unity3d.com/6000.2/Documentation/Manual/AudioMixer.html
- unity ile Sıfırdan Oyun Geliştirme Eğitimi: https://www.udemy.com/course/sifirdan-unity-ile-oyun-gelistirme-egitimi-unity-6/learn/lecture/48308199?start=0
- unity tutorial: https://unitycodemonkey.com/kitchenchaoscourse.php

---

## Ek Bilgiler
- **Platform:** Windows (x86_64)  
- **Sahne:** `ai_test_scene.unity`  
- **Checkpoint:** `CheckpointManager.Instance.PlayerDied()`  
- **Tema:** Sci-Fi, low-poly uzay üssü  
- **Mekanikler:** FSM, NavMesh, Health/Ammo UI, ışık/ses/müzik sistemi  

---

> *Bu proje, Kocaeli Üniversitesi Bilişim Sistemleri Mühendisliği 2025–2026 Güz dönemi Yazılım Geliştirme Laboratuvarı I dersi kapsamında geliştirilmiştir.*  
> © 2025 — İrem Kalaycı,Turkay Jafarli,Muhammed Taha Kızıkoğlu

---

## Menü & Ayarlar Sistemi *(eklenecek)*
*Bu bölüm, menü sistemini tamamladıktan sonra güncellenecektir.*
