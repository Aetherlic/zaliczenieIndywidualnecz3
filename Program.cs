using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;


namespace MusicLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            MusicLibrary library = new MusicLibrary();

            while (true)
            {
                Console.WriteLine("1. Naciśnij 1, aby dodać nową płytę do kolekcji do swojej bazy danych");
                Console.WriteLine("2. Naciśnij 2, aby wyświetlić wszystkie swoje płyty");
                Console.WriteLine("3. Naćiśnij 3, aby wyświetlić szczegółowe informacje o danej płycie");
                Console.WriteLine("4. Naciśnij 4, aby wyświetlić wszystkich wykonawców na danej płycie");
                Console.WriteLine("5. Naciśnij 5, aby wyświetlić szczegółowe informacje o danym utworze");
                Console.WriteLine("6. Naciśnij 6, aby zapisać tą bazę do pliku");
                Console.WriteLine("7. Naciśnij 7, aby odczytać tą bazę z pliku");
                Console.WriteLine("8. Naciśnij 8, aby zamknąć ten progam");

                Console.Write("\nNumer operacji: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        library.DodajPlyte();
                        break;
                    case "2":
                        library.WszystkiePlyty();
                        break;
                    case "3":
                        library.DetalePlyty();
                        break;
                    case "4":
                        library.Wykonawcy();
                        break;
                    case "5":
                        library.DetaleUtwor();
                        break;
                    case "6":
                        library.ZapiszPlik();
                        break;
                    case "7":
                        library.LadujPlik();
                        break;
                    case "8":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Coś takiego nie istnieje, spróbuj ponownie.");
                        break;
                }

                Console.WriteLine();
            }
        }
    }

    public class MusicLibrary
    {
        private List<CustomAlbum> albums;

        public MusicLibrary()
        {
            albums = new List<CustomAlbum>();
        }

        public void DodajPlyte()
        {
            Console.WriteLine("\n\nPraca nad dodaniem nowej płyty:");

            Console.Write("Tytuł płyty: ");
            string title = Console.ReadLine();

            string type;
            while (true)
            {
                Console.Write("Typ płyty (CD/DVD): ");
                type = Console.ReadLine();
                if (type.Equals("CD", StringComparison.OrdinalIgnoreCase) || type.Equals("DVD", StringComparison.OrdinalIgnoreCase))
                    break;
                else
                    Console.WriteLine("Error: zły typ płyty");
            }

            Console.Write("Wykonawca: ");
            string author = Console.ReadLine();

            Console.Write("Czas trwania (minuty.sekundy): ");
            string durationString = Console.ReadLine();
            double duration = double.Parse(durationString.Replace(',','.'),CultureInfo.InvariantCulture);

            Console.Write("ID(numer) płyty: ");
            int albumNumber = int.Parse(Console.ReadLine());

            CustomAlbum album = new CustomAlbum(title, type, duration, albumNumber, true);

            Console.WriteLine("Lista utworów:\n");

            string uSprawdz;
            do
            {
                Console.Write("Podaj tytuł utworu: ");
                string uName = Console.ReadLine();

                Console.Write("Podaj czas trwania utworu (minuty,sekundy): ");
                string uDurationString = Console.ReadLine();
                double uDuration = double.Parse(uDurationString.Replace(',','.'),CultureInfo.InvariantCulture);

                Console.Write("Podaj wykonawców utworu (jeżeli jest ich więcej użyj przecinka): ");
                string[] uArtists = Console.ReadLine().Split(',');

                Console.Write("Podaj kompozytora utworu: ");
                string uComposer = Console.ReadLine();

                Console.Write("Podaj numer utworu: ");
                int uNumber = int.Parse(Console.ReadLine());

                CustomTrack track = new CustomTrack(uName, uDuration, uComposer, uNumber, true);
                track.Performers.AddRange(uArtists);
                album.AddTrack(track);

                Console.Write("Czy na płycie jest więcej piosenek? T/N: ");
                uSprawdz = Console.ReadLine();
            } while (uSprawdz.ToUpper() == "T");

            albums.Add(album);

            Console.WriteLine("\nTa Płyta własnie została dodana do twojej bazy danych.");
        }

        public void WszystkiePlyty()
        {
            Console.WriteLine("\n\nLista wszystkich płyt:");

            foreach (var album in albums)
            {
                Console.WriteLine($"ID(Numer) płyty: {album.AlbumNumber}, Tytuł: {album.Title}");
            }
        }

        public void DetalePlyty()
        {
            Console.Write("Podaj ID(numer) płyty: ");
            int albumNumber = int.Parse(Console.ReadLine());

            var album = szukaj(albumNumber);

            if (album.Exists)
            {
                album.PokazAlbum();
            }
            else
            {
                Console.WriteLine("Płyta o podanym ID nie istnieje.");
            }
        }

        public void Wykonawcy()
        {
            Console.Write("Podaj ID(numer) płyty: ");
            int albumNumber = int.Parse(Console.ReadLine());

            var album = szukaj(albumNumber);

            if (album.Exists)
            {
                Console.WriteLine($"Wykonawcy na wybranej płycie {album.Title}:");
                foreach (var track in album.Tracks)
                {
                    Console.WriteLine($"Utwór: {track.Title} posiada następujących wykonawców:");
                    foreach (var performer in track.Performers)
                    {
                        Console.WriteLine($"- {performer}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Płyta o podanym ID nie istnieje.");
            }
        }

        public void DetaleUtwor()
        {
            Console.Write("Podaj numer płyty: ");
            int albumNumber = int.Parse(Console.ReadLine());

            var album = szukaj(albumNumber);

            if (album.Exists)
            {
                Console.Write("Podaj numer utworu: ");
                int trackNumber = int.Parse(Console.ReadLine());

                album.PokazUtwor(trackNumber);
            }
            else
            {
                Console.WriteLine("Płyta o podanym numerze nie istnieje.");
            }
        }

        private CustomAlbum szukaj(int albumNumber)
        {
            return albums.Find(album => album.AlbumNumber == albumNumber);
        }

        public void ZapiszPlik()
        {
            using (StreamWriter file = File.CreateText("ZapisanyAlbum.txt"))
            {
                foreach (var album in albums)
                {
                    file.WriteLine($"Title:{album.Title},Type:{album.Type},Duration:{album.Duration.ToString("0.0",CultureInfo.InvariantCulture)},AlbumNumber:{album.AlbumNumber}");

                    foreach (var track in album.Tracks)
                    {
                        file.WriteLine($"Track:{track.Title},{track.Duration.ToString("0.0",CultureInfo.InvariantCulture)},{track.Composer},{track.TrackNumber}");


                        foreach (var performer in track.Performers)
                        {
                            file.WriteLine($"Performer:{performer}");
                        }
                    }
                    file.WriteLine("---");
                }
            }

            Console.WriteLine("Baza danych została zapisana do pliku ZapisanyAlbum.txt.");
            Console.WriteLine("Plik znajduje się w C:\\Users\\ajola\\source\\repos\\zaliczenieIndywidualnecz3\\bin\\Debug\\net8.0");
        }

        public void LadujPlik()
        {
            if (File.Exists("ZapisanyAlbum.txt"))
            {
                albums.Clear();
                using (StreamReader file = File.OpenText("ZapisanyAlbum.txt"))
                {
                    CustomAlbum currentAlbum = new CustomAlbum();
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.StartsWith("Title:"))
                        {
                            string[] albumInfo = line.Split(',');
                            string title = albumInfo[0].Split(':')[1];
                            string type = albumInfo[1].Split(':')[1];
                            double duration = double.Parse(albumInfo[2].Split(':')[1],CultureInfo.InvariantCulture);
                            int albumNumber = int.Parse(albumInfo[3].Split(':')[1]);

                            currentAlbum = new CustomAlbum(title, type, duration, albumNumber, true);
                            albums.Add(currentAlbum);
                        }
                        else if (line.StartsWith("Track:"))
                        {
                            string[] trackInfo = line.Split(',');
                            string title = trackInfo[0].Split(':')[1];
                            double duration = double.Parse(trackInfo[1],CultureInfo.InvariantCulture);
                            string composer = trackInfo[2];
                            int trackNumber = int.Parse(trackInfo[3]);

                            currentAlbum.AddTrack(new CustomTrack(title, duration, composer, trackNumber, true));
                        }
                        else if (line.StartsWith("Performer:"))
                        {
                            string performer = line.Split(':')[1];
                            currentAlbum.AddPerformer(performer);
                        }
                        else if (line == "---")
                        {
                            currentAlbum = new CustomAlbum();
                        }
                    }
                }

                Console.WriteLine("Twoja Baza danych sukcesywnie została wczytana z pliku.");
            }
            else
            {
                Console.WriteLine("Error:Ten Plik z zapisaną bazą danych nie istnieje.");
            }
        }
    }

    public struct CustomAlbum
    {
        public string Title { get; }
        public string Type { get; }
        public double Duration { get; }
        public int AlbumNumber { get; }
        public List<CustomTrack> Tracks { get; }
        public List<string> Performers { get; }
        public bool Exists { get; }

        public CustomAlbum(string title, string type, double duration, int albumNumber, bool exists)
        {
            Title = title;
            Type = type;
            Duration = duration;
            AlbumNumber = albumNumber;
            Tracks = new List<CustomTrack>();
            Performers = new List<string>();
            Exists = exists;
        }

        public void AddTrack(CustomTrack track)
        {
            Tracks.Add(track);
        }

        public void AddPerformer(string performer)
        {
            Performers.Add(performer);
        }

        public void PokazAlbum()
        {
            Console.WriteLine($"Album title: {Title}");
            Console.WriteLine($"Type: {Type}");
            Console.WriteLine($"Duration: {Duration} minutes");
            Console.WriteLine($"Album number: {AlbumNumber}");
            Console.WriteLine("Track list:");

            foreach (var track in Tracks)
            {
                Console.WriteLine($"- {track.TrackNumber}: {track.Title}");
            }
        }

        public void PokazUtwor(int trackNumber)
        {
            var track = Tracks.Find(t => t.TrackNumber == trackNumber);
            if (track.Exists)
            {
                Console.WriteLine($"Track title: {track.Title}");
                Console.WriteLine($"Duration: {track.Duration} minutes");
                Console.WriteLine("Performers:");
                foreach (var performer in track.Performers)
                {
                    Console.WriteLine($"- {performer}");
                }
                Console.WriteLine($"Composer: {track.Composer}");
            }
            else
            {
                Console.WriteLine("Taki utwór nie istnieje, sprawdz czy ID utworu jest poprawne.");
            }
        }

    }

    public class Track
    {
        public string Title { get; }
        public double Duration { get; }
        public List<string> Performers { get; }
        public string Composer { get; }
        public int TrackNumber { get; }
        public bool Exists { get; }

        public Track(string title, double duration, string composer, int trackNumber, bool exists)
        {
            Title = title;
            Duration = duration;
            Performers = new List<string>();
            Composer = composer;
            TrackNumber = trackNumber;
            Exists = exists;
        }
    }

    public class CustomTrack : Track
    {
        public CustomTrack(string title, double duration, string composer, int trackNumber, bool exists)
            : base(title, duration, composer, trackNumber, exists) { }
    }
}

