namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            //02.
            //string result = ExportAlbumsInfo(context, 9);
            //Console.WriteLine(result);

            //03.
            string result1 = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(result1);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albumsInfo = context.Albums
                                    .ToArray()
                                    .Where(a => a.ProducerId == producerId)
                                    .OrderByDescending(a => a.Price)
                                    .Select(a => new
                                    {
                                        AlbumName = a.Name,
                                        ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                        ProducerName = a.Producer.Name,
                                        Songs = a.Songs
                                                 .ToArray()
                                                 .Select(s => new
                                                 {
                                                     SongName = s.Name,
                                                     Price = s.Price.ToString("f2"),
                                                     Writer = s.Writer.Name
                                                 })
                                                 .OrderByDescending(s => s.SongName)
                                                 .ThenBy(s => s.Writer)
                                                 .ToArray(),
                                        TotalAlbumPrice = a.Price.ToString("f2")
                                    })
                                    .ToArray();

            foreach (var album in albumsInfo)
            {
                sb.Append($"-AlbumName: {album.AlbumName}\r\n")
                  .Append($"-ReleaseDate: {album.ReleaseDate}\r\n")
                  .Append($"-ProducerName: {album.ProducerName}\r\n")
                  .Append($"-Songs:\r\n");

                int i = 1;
                foreach (var song in album.Songs)
                {
                    sb.Append($"---#{i++}\r\n")
                      .Append($"---SongName: {song.SongName}\r\n")
                      .Append($"---Price: {song.Price}\r\n")
                      .Append($"---Writer: {song.Writer}\r\n");
                }

                sb.Append($"-AlbumPrice: {album.TotalAlbumPrice}\r\n");
            };

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songsInfo = context.Songs
                                   .ToArray()
                                   .Where(s => s.Duration.TotalSeconds > duration)
                                   .Select(s => new
                                   {
                                       SongName = s.Name,
                                       Performer = s.SongPerformers
                                                    .ToArray()
                                                    .Select(sp=> $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                                                    .FirstOrDefault(),
                                       Writer = s.Writer.Name,
                                       AlbumProducer = s.Album.Producer.Name,
                                       Duration = s.Duration.ToString("c",CultureInfo.InvariantCulture)
                                   })
                                   .OrderBy(s => s.SongName)
                                   .ThenBy(s=>s.Writer)
                                   .ThenBy(s=>s.Performer)
                                   .ToArray();

            int i = 1;

            foreach (var song in songsInfo)
            {
                sb.Append($"-Song #{i++}\r\n")
                  .Append($"---SongName: {song.SongName}\r\n")
                  .Append($"---Writer: {song.Writer}\r\n")
                  .Append($"---Performer: {song.Performer}\r\n")
                  .Append($"---AlbumProducer: {song.AlbumProducer}\r\n")
                  .Append($"---Duration: {song.Duration}\r\n");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
