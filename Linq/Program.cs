using CsvHelper.Configuration.Attributes;
using Linq;
using System.Formats.Asn1;
using System.Globalization;

namespace Linq
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = @"C:\Users\igoss\source\repos\LINQ with CSV\Linq\googleplaystore1.csv";
            var googleApps = LoadGoogleAps(csvPath);

            //Display(googleApps);
            //GetData(googleApps);
            //ProjectData(googleApps);
            //DivideData(googleApps);
            //OrderData(googleApps);
            //GroupData(googleApps);
            GroupDataOperations(googleApps);

        }

        // pobieranie danych LINQ
        static void GetData(IEnumerable<GoogleApp> googleApps)
        {
            //bool isHighRatedApp(GoogleApp app)
            //{
            //    return app.Rating > 4.6;
            //}
            //var highRatedBeautyApps = googleApps.Where(isHighRatedApp);

            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);

            Display(highRatedBeautyApps);

            var firstHighRatedBeautyApp = highRatedBeautyApps.FirstOrDefault(app => app.Reviews > 300);

            // .SingleOrDefault sprawdzi dodatkowo czy istnieje tylko jeden element, jeśli nie - wyjątek
            // .LastOrDefault wyświetli ostatni element spełniający warunek 

            Console.WriteLine("firstHighRatedBeautyApp:");
            Console.WriteLine(firstHighRatedBeautyApp);

        }

        // przekształcanie danych wyjściowych na inne wyjściowe LINQ
        static void ProjectData(IEnumerable<GoogleApp> googleApps)
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);

            var highRatedBeautyAppsNames = highRatedBeautyApps.Select(app => app.Name);

            //Console.WriteLine(string.Join(',', highRatedBeautyAppsNames));

            // tworzenie nowego obiektu klasy GoogleAppDto
            var dtos = highRatedBeautyApps.Select(app => new GoogleAppDto()
            {
                Name = app.Name,
                Reviews = app.Reviews
            });

            foreach (var dto in dtos)
            {
                Console.WriteLine($"{dto.Name} : {dto.Reviews}");
            }

            // .SelectMany jeśli mamy jakąś liste jako pole klasy
            var genres = highRatedBeautyApps.SelectMany(app => app.Genres);

            // typy anonimowe, bez tworzenia nowego obiektu
            var anonymousDtos = highRatedBeautyApps.Select(app => new
            {
                Reviews = app.Reviews,
                Name = app.Name,
                Category = app.Category
            });
        }

        // dzielenie danych LINQ
        static void DivideData(IEnumerable<GoogleApp> googleApps)
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);

            var first5HighRatedBeautyApps = highRatedBeautyApps.Take(5);

            // .TakeLast zwraca ostatnie n elementów
            // .TakeWhile zwraca elementy dopóki warunek będzie nieprawdziwy
            // .Skip pomija pierwsze n elementów

            Display(first5HighRatedBeautyApps);

        }

        // sortowanie danych LINQ
        static void OrderData(IEnumerable<GoogleApp> googleApps)
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);

            var sortedResults = highRatedBeautyApps.OrderByDescending(app => app.Rating).ThenBy(app => app.Name);

            Display(sortedResults);

        }

        // grupowanie danych LINQ
        static void GroupData(IEnumerable<GoogleApp> googleApps)
        {
            var categoryGroup = googleApps.GroupBy(app => app.Category);

            // grupowanie po konkretnej kategorii

            //var artAndDesignGroup = categoryGroup.First(e => e.Key == Category.ART_AND_DESIGN);

            //var apps = artAndDesignGroup.Select(app => app); lub var apps = artAndDesignGroup.toList();

            // Display(apps)

            foreach (var group in categoryGroup) // grupowanie po każdej kategorii
            {
                var apps = group.ToList();

                Console.WriteLine($"Displaying elements for a group {group.Key}");

                Display(apps);
            }
        }

        static void GroupDataOperations(IEnumerable<GoogleApp> googleApps)
        {
            var categoryGroup = googleApps
                .GroupBy(app => app.Category)
                .Where(g => g.Min(g => g.Reviews) >= 10);

            foreach (var group in categoryGroup)
            {
                var averageReviews = group.Average(g => g.Reviews);
                var minReviews = group.Min(g => g.Reviews);
                var maxReviews = group.Max(g => g.Reviews);
                var reviewsCount = group.Sum(g => g.Reviews);

                // czy wszystkie app w grupie mają rating > 3.0
                var allAppsHaveRatingOfThree = group.All(g => g.Rating > 3.0);

                Console.WriteLine($"Group: {group.Key}");
                Console.WriteLine($"averageReviews: {averageReviews}");
                Console.WriteLine($"minReviews: {minReviews}");
                Console.WriteLine($"maxReviews: {maxReviews}");
                Console.WriteLine($"reviewsCount: {reviewsCount}");
                Console.WriteLine();

            }
        }

        static void Display(IEnumerable<GoogleApp> googleApps)
        {
            foreach (var googleApp in googleApps)
            {
                Console.WriteLine(googleApp);
            }

        }
        static void Display(GoogleApp googleApp)
        {
            Console.WriteLine(googleApp);
        }

        static List<GoogleApp> LoadGoogleAps(string csvPath)
        {
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<GoogleAppMap>();
                var records = csv.GetRecords<GoogleApp>().ToList();
                return records;
            }

        }

    }


}
