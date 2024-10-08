using System.Diagnostics;
using System.Xml.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        //string url = "https://www.federalregister.gov/documents/full_text/text/2024/07/25/2024-16227.txt";
        //string url = "https://www.federalregister.gov/documents/full_text/text/2024/08/30/2024-14824.txt";
        string url = "https://www.federalregister.gov/documents/full_text/text/2024/08/30/2024-14824.txt";


        string filePath = "C:\\Temp\\downloaded_file.txt";

        // Call the method to download and save the file
        await DownloadFileAsync(url, filePath);

        // Read the downloaded file's text content
        string fileContent = await ReadFileContentAsync(filePath);

        // Output the text content
        //Console.WriteLine(fileContent);

        //CountWordsInFile(filePath);

        //FindChaptersInFile(filePath);

        List<int> pageNumbers = FindThePageNumbers();

        FindSectionsInFile(filePath, pageNumbers);
    }

    // Method to download the file and save it to disk
    public static async Task DownloadFileAsync(string url, string filePath)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Download the file content as a string
                string content = await client.GetStringAsync(url);

                // Save the content to a file
                await File.WriteAllTextAsync(filePath, content);

                Console.WriteLine($"File downloaded and saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    // Method to read the content of the saved file
    public static async Task<string> ReadFileContentAsync(string filePath)
    {
        try
        {
            // Read all the text from the file asynchronously
            string content = await File.ReadAllTextAsync(filePath);

            Console.WriteLine("File content read successfully.");
            return content;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
            return string.Empty;
        }
    }


    static async Task ReadXmlWithXDocument()
    {
        string url = "https://www.federalregister.gov/documents/full_text/xml/2024/09/12/2024-20616.xml";

        // Download the XML content from the URL
        using (HttpClient client = new HttpClient())
        {
            string xmlContent = await client.GetStringAsync(url);

            // Load the XML into XDocument
            XDocument xdoc = XDocument.Parse(xmlContent);

            // Extract and print all text from leaf nodes (nodes without children)
            var textNodes = xdoc.Descendants().Where(x => !x.HasElements).Select(x => x.Value);

            foreach (var text in textNodes)
            {
                Console.WriteLine(text);
            }
        }
    }
    //ReadXmlWithXmlDocument()

    static void CountWordsInFile(string filePath)
    {
        try
        {
            // Start the timer
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Read the file content
            string content = File.ReadAllText(filePath);

            // Count the number of words in the file
            int wordCount = CountWords(content);

            // Stop the timer
            stopwatch.Stop();

            Console.WriteLine($"Word count: {wordCount}");
            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static int CountWords(string content)
    {
        // Check if the content is null or empty
        if (string.IsNullOrEmpty(content))
            return 0;

        // Split the content by spaces, tabs, or newlines and count words
        string[] words = content.Split(new char[] { ' ', '\t', '\n', '\r', '-' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    static void FindChaptersInFile(string filePath)
    {
        try
        {
            // Start the timer
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Read the file content
            string content = File.ReadAllText(filePath);

            // Count the number of words in the file
            string[] chapters = FindChapters(content);

            // Stop the timer
            stopwatch.Stop();

            // Iterate through the chapters and display them
            for (int i = 0; i < chapters.Length; i++)
            {
                Console.WriteLine($"Chapter {i + 1}:");
                Console.WriteLine(chapters[i]);
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("---------------------------------------------");

            }

            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static string[] FindChapters(string content)
    {
        // Check if the content is null or empty
        if (string.IsNullOrEmpty(content))
            return null;

        // Define the delimiter
        string delimiter = "<HD SOURCE=\"HD1\">";

        // Split the content by the delimiter
        string[] chapters = content.Split(new string[] { delimiter }, StringSplitOptions.None);

        return chapters;
    }

    static void FindSectionsInFile(string filePath, List<int> pageNumbers)
    {
        try
        {
            // Start the timer
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Read the file content
            string content = File.ReadAllText(filePath);

            // Count the number of words in the file
            string[] sections = FindGivenPageNumberSections(content, pageNumbers);

            // Stop the timer
            stopwatch.Stop();

            // Iterate through the chapters and display them
            for (int i = 0; i < sections.Length; i++)
            {
                Console.WriteLine($"Section {i + 1}:");
                Console.WriteLine(sections[i]);
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("---------------------------------------------");
                Console.ReadLine();
            }

            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static string[] FindGivenPageNumberSections(string content, List<int> pageNumbers)
    {
        // Check if the content is null or empty
        if (string.IsNullOrEmpty(content))
            return null;

        // Define the delimiters
        List<string> thePageDelimitersList = new List<string>();
        foreach (int i in pageNumbers)
        {
            thePageDelimitersList.Add($"[[Page {i}]]");
        }

        // Split the content by the delimiter
        string[] sections = content.Split(thePageDelimitersList.ToArray(), StringSplitOptions.None);

        return sections;
    }

    static List<int> FindPages(int x, int y, int z)
    {
        List<int> pageNumbers = new List<int>();

        // Base case: if the difference between y and x is smaller than z
        if (y - x < z)
        {
            pageNumbers.Add(x);
            pageNumbers.Add(y);
        }
        else
        {
            // Recursive case: divide the range [x, y] into halves
            int mid = (x + y) / 2;

            // Recursively find pages for the left half and right half
            List<int> leftPages = FindPages(x, mid, z);
            List<int> rightPages = FindPages(mid + 1, y, z);

            // Combine the results
            pageNumbers.AddRange(leftPages.GetRange(0, leftPages.Count - 1));  // Add all but the last element of leftPages
            pageNumbers.AddRange(rightPages);  // Add all of rightPages
        }

        return pageNumbers;
    }

    static List<int> FindThePageNumbers()
    {
        int x = 70698;
        int y = 71073;
        int z = 60;

        List<int> result = FindPages(x, y, z);

        // Print the result
        Console.WriteLine(string.Join(", ", result));
        Console.ReadLine();

        return result;
    }
}





