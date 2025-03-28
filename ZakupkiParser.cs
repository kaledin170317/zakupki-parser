using System.Collections.Concurrent;

public class ParserHTML(HttpClient client)
{
    private const string Url = "https://zakupki.gov.ru/epz/order/notice/printForm/view.html?regNumber=";
    private const string Dir = @"C:\Users\kaled\RiderProjects\ZakupkiParser\ZakupkiParser\html\";

    private async Task DownloadAndSaveHtml(string id)
    {
        var file = Path.Combine(Dir, id + ".html");
        if (!File.Exists(file))
        {
            var url = Url + id;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var html = await response.Content.ReadAsStringAsync();
                    await File.WriteAllTextAsync(file, html);
                    Console.WriteLine($"Cохранен: {file}");
                }
                else
                {
                    Console.WriteLine($"[{response.StatusCode}] Страница не найдена: {url}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке {id}: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Уже существует: {file}");
        }
    }

    public async Task DownloadAndSaveHtml(IEnumerable<string> ids)
    {
        var tasks = ids.Select(DownloadAndSaveHtml).ToList();
        await Task.WhenAll(tasks);
    }
    
    private async Task<string[]> GetDownloadedIdsAsync()
    {
        if (!Directory.Exists(Dir))
        {
            Console.WriteLine("Папка не найдена.");
            return [];
        }

        return await Task.Run(() =>
        {
            return Directory
                .GetFiles(Dir, "*.html")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray()!;
        });
    }

    public async Task<ConcurrentDictionary<string, string>> LoadFileContentsAsync()
    {
        var fileIds = await GetDownloadedIdsAsync();
        var result = new ConcurrentDictionary<string, string>();

        var tasks = fileIds.Select(async id =>
        {
            var path = Path.Combine(Dir, id + ".html");
            
            try
            {
                var content = await File.ReadAllTextAsync(path);
                result.TryAdd(id, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла {id}: {ex.Message}");
            }
        });

        await Task.WhenAll(tasks);

        return result;
    }
}