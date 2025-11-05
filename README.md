Yapay Zeka Destekli TPS-Third Person Shooter Oyunu

Kocaeli Üniversitesi Teknoloji Fakültesi Bilişim Sistemleri Mühendisliği Yazılım Geliştirme Laboratuvarı I — 2025–2026 Güz Dönemi

İrem Kalaycı-231307047:Ses, müzik,AI, NPC sistemle, sahne tasarımı
Muhammed Taha Kızıkoğlu-241307121: UI/UX, menü sistemleri,sahne tasarımı
Turkay Jafarli-221307112: optimizasyon ve ışıklandırma,sahne tasarımı,player mekanikleri

Proje Özeti

Ay yüzeyinde geçen üçüncü şahıs nişancı TPS türündeki bu oyunda oyuncu, uzay istasyonu çevresinde görev yaparken düşmanlarla çatışır.
Oyun, FSM tabanlı yapay zekâya sahip NPC sistemleri, NavMesh tabanlı pathfinding, dinamik ışıklandırma, menü ve ayar arayüzleri ile desteklenmiştir.

Oyuncu; koşma, zıplama, nişan alma, ateş etme, taktiksel pozisyon alma, şarjör değiştirme gibi tüm temel TPS mekaniklerini kullanabilir.
Düşmanlar; farklı davranış biçimlerine sahip canavar, zıplayarak saldıran “Ely by K.Atienza” adlı yakın dövüşçü ve menzilli asker NPC’lerden oluşur.

Senaryo

Yıl 2097. Ay’daki “Helios” araştırma istasyonundan gelen acil kod, bölgede tanımlanamayan yaratıklar ve sabotaj yapan düşman askerlerinin varlığını bildirir.
Oyuncu, istasyonun savunmasını üstlenen son güvenlik birimidir.

Görev:

İstasyondaki saldırganları etkisiz hale getirmek,

Reaktör odasını korumak,

İletişim antenini yeniden etkinleştirmek.

Oyun dünyasında:

2 bilim kurgu ofis alanı,

Açık kraterli ay yüzeyi,

Uçan UFO trafiği,

Gelişmiş ışık ve gölge atmosferi yer alır.

Oyun Mekanikleri

Oyuncu (Player)

Hareket: W, A, S, D (yürüme/koşma), Shift (hızlanma), Space (zıplama)

Nişan & Ateş: Sağ tık nişan, sol tık ateş (yarı otomatik/sürekli)

Şarjör Değiştirme: R ile, animasyon senkronlu UI güncellemesi

Sağlık Sistemi: Hasar alımı, ölüm animasyonu ve health bar UI

Mermi Göstergesi (Ammo UI): Ekranda aktif silahın mermi bilgisi

Ses/Müzik: Adım, silah, hasar ve arka plan müziği

Menü & UI:

Ana Menü: Play / Settings / Quit

Settings: Master, Music, SFX ayarı
NPC Yapay Zekâ Sistemi (FSM + NavMesh)
1. Genel Yapı

FSM (Finite State Machine): Her NPC duruma (Idle, Patrol, Chase, Attack, Death) göre hareket eder.

Algı: Görüş konisi (FOV), mesafe, raycast tabanlı engel kontrolü.

Pathfinding: Unity NavMesh Agent ile yol bulma.

Patrol Sistemi: NPC’ler belirlenen PatrolPoint’ler arasında gezinir.

Hasar ve Ölüm: Her NPC kendi health sistemine sahip olup ölüm animasyonu sonrası devre dışı kalır.

2. NPC Türleri
Canavar NPC (Melee Beast)

Oyuncuya yakın dövüş hasarı verir.

FSM Durumları:
Idle → Patrol → Chase → Attack → Cooldown → Patrol

Parametreler:

Görüş Mesafesi: 18

Hasar: 15

Hız: 4

Attack cooldown: 1.6s

Devriye sırasında oyuncuyu fark eder etmez kovalamaya geçer.

Yakın menzilde Attack animasyonu tetiklenir.

Pause: Resume / Settings / Quit to Menu

Ely By K.Atienza – Jump-Attack NPC (Assassin)

[Entry] → [Walking] ↔ [Running]
[Walking] → [Jump Attack]
[Any State] → [Falling Back Death] → [Exit]

vInput	Float	Dikey eksen hareket girdisi
hzInput	Float	Yatay eksen hareket girdisi
Attack	Bool	Jump Attack başlatır
FallingBack	Bool	Ölüm animasyonunu tetikler


FSM Geçişleri:

Oyuncu algılandığında → Running

Oyuncu yakınsa → Jump Attack

Saldırı tamamlandığında → Walking

Sağlık sıfırsa → Falling Back Death

Özellikler:

Jump Attack sırasında Root Motion aktif → zıplama yönü animasyondan alınır.

Hasar kutusu (Trigger Collider) yalnızca animasyonun ortasında etkin olur.

İniş sonrası kısa “stun” süresi eklenmiştir.

Ölüm sonrası NavMeshAgent ve collider devre dışı kalır.

Asker NPC (Ranged Soldier)
FSM Durumları:
Idle → Patrol → Chase → TakeCover → Aim → Shoot → Reload → Cooldown


UFO Sistemi

Uçan UFO’lar spline/waypoint sistemiyle hareket eder.
Görevleri:

Görsel atmosfer oluşturmak,

Işık efektleriyle sahne dinamizmi kazandırmak.

Uzaktan LOD optimizasyonu uygulanmıştır.

Grafik, Işıklandırma ve Performans

Low Poly Sci-Fi teması.

Dış mekân: Directional Light (ay ışığı) + Post-Processing (bloom, vignette).

İç mekân: Mixed Lighting + baked GI.

LOD, statik batching, object pooling ile 60 FPS hedeflenmiştir.

Ses & Müzik

AudioMixer: Master, Music, SFX grupları.

Ayarlar menüsünden ses seviyesi kontrol edilir.

Oyun içi efektler: silah sesi,jump attack, ölüm efekti.


Kullanılan Teknolojiler

Unity 6000.2.7f2

C#

NavMesh / AI Navigation

TextMeshPro

Cinemachine

Post-Processing

ScriptableObject (NPC ve Silah konfigleri)

GitHub (sürüm kontrol)
est Edilen Özellikler:

FSM geçişleri

Jump Attack event zamanlaması

NavMesh path doğruluğu

UI senkronizasyonu (Health, Ammo, Pause)

FPS stabilitesi (Profiler ile 60 FPS)


Build Bilgisi

Platform: Windows (x86_64)

Sahne: Level_AyIstasyonu.unity

Kontroller:

W, A, S, D → hareket

Shift → koşma

Space → zıplama

RMB → nişan

LMB → ateş

R → reload

Esc → pause

| Karşılaşılan Sorun                             | Çözüm                                                                   |
| ---------------------------------------------- | ----------------------------------------------------------------------- |
| Jump Attack’ta NavMeshAgent kontrolü bozulması | RootMotion sırasında `updatePosition=false`, iniş sonrası yeniden aktif |
| NPC’lerin bazen oyuncuyu görememesi            | Raycast maskesi yeniden yapılandırıldı                                  |
| Mermi havuzu optimizasyonu                     | Object pooling sistemi kuruldu                                          |
| UI senkronizasyonu                             | Event bazlı UI güncellemesi (Observer Pattern)                          |


Kazanımlar

FSM tabanlı yapay zekâ kurgulama

NavMesh pathfinding uygulaması

AudioMixer & UI senkronizasyonu

Optimizasyon ve ışık düzenleme

GitHub ile ekip temelli sürüm yönetimi


Sonuç

Bu proje; temel TPS mekaniklerinin tümünü içeren, FSM tabanlı yapay zekâ sistemlerine sahip, oynanabilir bir üçüncü şahıs nişancı oyunudur.
Görsel, işitsel ve teknik yönleriyle tamamlanmış olan bu çalışma, Yazılım Geliştirme Laboratuvarı – I dersi için tüm isterleri karşılamaktadır.

Kaynakça

Unity AI Navigation Documentation

Unity Animator & FSM Design Guide

Game Programming Patterns-Robert Nystrom

Mixamo Animation Library

Unity AudioMixer Reference
