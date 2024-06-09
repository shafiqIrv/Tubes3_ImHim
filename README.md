# Tubes3_ImHim

## Deskripsi Tugas

Algoritma Knuth-Morris-Pratt (KMP) dan Boyer-Moore (BM) adalah teknik pencocokan pola yang efisien untuk mencari keberadaan suatu pola dalam sebuah teks. Dalam program ini, KMP menggunakan tabel lompatan untuk menghindari pencocokan karakter yang tidak perlu, sementara BM memanfaatkan heuristik bad character dan good suffix untuk mengoptimalkan pergeseran pola. Regex, singkatan dari regular expression, adalah suatu hal yang digunakan untuk memanipulasi string berdasarkan pola yang kompleks, memberikan fleksibilitas dalam pencarian dan manipulasi string. Dalam program ini, regex digunakan untuk mencocokkan nama yang mungkin mengalami variasi seperti perubahan huruf besar atau kecil, penggunaan huruf menjadi angka, atau hilangnya huruf vokal.

## Requirement program

1. .NET Core SDK
2. MySQL Server

## Cara Kompilasi

1. Inisialisasi database

    Jalankan service mysql jika belum di start, buat database, dan lakukan insersi ke database tersebut lalu ubah file connectionSettings.json sesuai dengan username, password, dan nama database yang dibuat.

    ```bash
    sudo service mysql start
    mysql -u [nama user] -p [nama basis data] < [nama file].sql
    ```

2. Inisialisasi aplikasi

    ```bash
    cd src/Tubes3_ImHim/Tubes3_ImHim
    dotnet restore
    dotnet build
    dotnet run
    ```

    Command-command tersebut akan menginisialisasi packages yang digunakan pada program beserta melakukan proses build lalu menjalankan program.

## Anggota

| NAMA ANGGOTA               | NIM      |
|----------------------------|----------|
| Shafiq Irvansyah           | 13522003 |
| Sa'ad Abdul Hakim          | 13522092 |
