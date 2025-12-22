# Simulasi Wawancara Kerja Berbasis Teks dengan Penilaian Machine Learning (SIWATE)

**SIWATE** adalah aplikasi web modern yang dirancang untuk membantu pelamar kerja berlatih wawancara. Aplikasi ini mensimulasikan sesi wawancara berbasis teks dan menggunakan kecerdasan buatan (Machine Learning) untuk memberikan penilaian skor dan umpan balik (*feedback*) otomatis terhadap jawaban pengguna.

---

## 📋 Daftar Isi
1. [Deskripsi Umum](#-deskripsi-umum)
2. [Fitur Utama](#-fitur-utama)
3. [Arsitektur Sistem](#-arsitektur-sistem)
4. [Machine Learning](#-machine-learning)
5. [Teknologi](#-teknologi)
6. [Struktur Project](#-struktur-project)
7. [Database (Supabase)](#-database-supabase)
8. [Instalasi & Cara Menjalankan](#-instalasi--cara-menjalankan)
9. [Catatan Penting](#-catatan-penting)

---

## 📖 Deskripsi Umum

Dalam dunia kerja yang kompetitif, persiapan wawancara adalah kunci. SIWATE hadir sebagai platform latihan mandiri di mana pengguna dapat:
1.  Menjawab pertanyaan wawancara yang umum ditanyakan HRD.
2.  Mendapatkan skor relevansi jawaban secara instan (0-100).
3.  Menerima saran perbaikan untuk meningkatkan kualitas jawaban.

Sistem ini dibangun dengan **ASP.NET Core MVC** untuk performa tinggi dan **ML.NET** untuk kemampuan analisis cerdas tanpa bergantung pada API pihak ketiga yang mahal.

---

## 🚀 Fitur Utama

### 👨‍💼 Modul User (Pelamar)
*   **Simulasi Wawancara**: Menjawab pertanyaan acak dari database.
*   **Penilaian Otomatis**: Mendapatkan skor dan feedback langsung setelah submit.
*   **Riwayat Latihan**: Melihat kembali hasil simulasi sebelumnya untuk melacak kemajuan.
*   **Antarmuka Modern**: Desain responsif dan bersih dengan Tailwind CSS.

### 👮 Modul Admin (Dashboard)
*   **Manajemen Pertanyaan**: Menambah, mengedit, atau menghapus bank soal wawancara.
*   **Manajemen Dataset**: Mengelola data latih (jawaban + skor) untuk *retraining* model.
*   **Training Model**: Melatih ulang model Machine Learning secara *offline* langsung dari dashboard admin.

---

## 🏗 Arsitektur Sistem

Sistem ini menggunakan arsitektur **MVC (Model-View-Controller)** yang terstruktur:

*   **Frontend**: Razor Views (.cshtml) dengan styling **Tailwind CSS**.
*   **Backend**: ASP.NET Core 9.0 (C#).
*   **Database**: PostgreSQL (di-hosting di **Supabase**), diakses menggunakan Entity Framework Core (EF Core).
*   **AI Engine**: ML.NET Library yang berjalan *in-process* (embedded) dalam aplikasi web.

---

## 🤖 Machine Learning

Aplikasi ini menggunakan framework **ML.NET** dari Microsoft.

### Spesifikasi Model
*   **Algorithm**: **Online Gradient Descent (SDCA Regression)**. Algoritma regresi linier yang efisien untuk memprediksi nilai numerik (skor).
*   **Task**: Regression (Prediksi Skor 0-100).
*   **Training Process**: Offline training (dipicu oleh Admin). Sistem membaca data dari tabel `datasets`, melatih model, dan menyimpan hasilnya sebagai file `interview_model.zip`.

### Feature Engineering
Model tidak membaca teks mentah begitu saja. Jawaban pengguna diproses melalui pipeline ekstraksi fitur:
1.  **FeaturizeText ("AnswerText")**: Mengubah teks jawaban menjadi vektor numerik menggunakan teknik **TF-IDF** (Term Frequency-Inverse Document Frequency). Ini membantu model memahami kata-kata kunci penting.
2.  **Concatenate**: Menggabungkan fitur teks dengan fitur tambahan (misal: panjang karakter) untuk akurasi yang lebih baik.

### Alur Prediksi
1.  User submit jawaban (String).
2.  Backend memuat model dari `interview_model.zip`.
3.  Model memprediksi angka (Score).
4.  Backend mengonversi Score menjadi Feedback naratif (misal: >80 = "Luar Biasa").

---

## 💻 Teknologi

*   **Framework**: ASP.NET Core 9.0
*   **Language**: C#
*   **Database**: PostgreSQL 15 (Supabase)
*   **ORM**: Entity Framework Core 9.0
*   **Machine Learning**: ML.NET 4.0
*   **CSS Framework**: Tailwind CSS (via CDN)
*   **IDE**: Visual Studio Code / Visual Studio 2022

---

## 📂 Struktur Project

```
Siwate.Web/
├── Controllers/        # Logika Bisnis (Interview, Admin, Account)
├── Data/              # Konfigurasi EF Core (DbContext)
├── Models/            # Representasi Tabel Database (User, Question, dll)
├── Services/          # Logika ML (MachineLearningService.cs)
├── Views/             # Tampilan Antarmuka (Razor Pages)
├── wwwroot/           # File Statis (CSS, JS, Images)
├── appsettings.json   # Konfigurasi Database & Environment
├── Program.cs         # Entry Point & Dependency Injection
└── interview_model.zip # File Model ML hasil training
```

---

## 🗄 Database (Supabase)

Proyek ini menggunakan **Supabase** sebagai layanan Backend-as-a-Service (BaaS) untuk database PostgreSQL.

**Tabel Utama:**
1.  `users`: Menyimpan data akun (Admin/User).
2.  `questions`: Bank soal wawancara.
3.  `answers`: Jawaban yang disubmit user.
4.  `interview_results`: Hasil penilaian (Skor & Feedback).
5.  `datasets`: Data latih untuk Machine Learning.

---

## 🛠 Instalasi & Cara Menjalankan

### Prasyarat
*   .NET SDK 9.0 (atau lebih baru).
*   Akun **Supabase** (untuk database).

### Langkah-langkah

1.  **Clone Repository**
    ```bash
    git clone https://github.com/username/SIWATE.git
    cd SIWATE/Siwate.Web
    ```

2.  **Konfigurasi Database**
    *   Buat project baru di Supabase.
    *   Jalankan script SQL yang tersedia di `database_setup.sql` pada SQL Editor Supabase untuk membuat tabel dan data awal.
    *   Buka `appsettings.json` dan sesuaikan koneksi database Anda:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Host=db.projectref.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=PASSWORD_ANDA"
        }
        ```

3.  **Restore Dependency**
    ```bash
    dotnet restore
    ```

4.  **Jalankan Aplikasi**
    ```bash
    dotnet run
    ```
    Buka browser dan akses `http://localhost:5xxx`.

5.  **Login Awal (Admin)**
    *   Email: `admin@siwate.com`
    *   Password: `admin`

---

## ⚠️ Catatan Penting

1.  **Training Model Pertama Kali**:
    Saat pertama kali dijalankan, model mungkin belum akurat atau belum ada.
    *   Login sebagai Admin.
    *   Masuk ke menu **Dataset**.
    *   Pastikan ada minimal 5 data.
    *   Klik **"Latih Model Sekarang"** agar file `interview_model.zip` terbentuk.

2.  **Keamanan**:
    Password user saat ini disimpan dalam bentuk *plain-text* untuk tujuan simulasi/pembelajaran. Untuk produksi, **WAJIB** menggunakan hashing (BCrypt/Argon2).

---

## 📄 Lisensi

Proyek ini dibuat untuk tujuan tugas besar perkuliahan.

---
*Dibuat dengan ❤️ oleh Tim Pengembang SIWATE*
