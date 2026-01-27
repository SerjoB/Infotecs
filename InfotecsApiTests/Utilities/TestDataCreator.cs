using System.Text;

namespace InfotecsApiTests.Utilities;

public static class TestDataCreator    // Creates data in bytes and wraps it in multipart format for our ImportController to upload
{
    public static byte[] CreateValidCsvBytes()
    {
        var csv = @"Date;ExecutionTime;Value
2024-01-15T10:00:00.0000Z;5.2;150.5
2024-01-15T10:05:00.0000Z;6.1;175.3
2024-01-15T10:10:00.0000Z;5.8;162.7
2024-01-15T10:15:00.0000Z;7.2;198.4
2024-01-15T10:20:00.0000Z;6.5;185.2";

        return Encoding.UTF8.GetBytes(csv);
    }

    public static byte[] CreateInvalidCsvBytes_TooManyRows()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Date;ExecutionTime;Value");
        
        for (int i = 0; i < 10001; i++)
        {
            sb.AppendLine($"2024-01-15T10:00:00.0000Z;5.0;100.0");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public static byte[] CreateInvalidCsvBytes_NegativeValue()
    {
        var csv = @"Date;ExecutionTime;Value
2024-01-15T10:00:00.0000Z;5.2;-150.5";

        return Encoding.UTF8.GetBytes(csv);
    }
    

    public static byte[] CreateInvalidCsvBytes_InvalidDateFormat()
    {
        var csv = @"Date;ExecutionTime;Value
2024-12-31-10:00:00.0000Z;5.2;150.5";

        return Encoding.UTF8.GetBytes(csv);
    }
    
    public static byte[] CreateInvalidCsvBytes_OutOfBoundForwardDate()
    {
        var data = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        var csv = "Date;ExecutionTime;Value \n" + data;

        return Encoding.UTF8.GetBytes(csv);
    }
    
    public static byte[] CreateInvalidCsvBytes_OutOfBoundBackwardDate()
    {
        var csv = @"Date;ExecutionTime;Value
1999-12-31-10:00:00.0000Z;5.2;150.5";

        return Encoding.UTF8.GetBytes(csv);
    }

    public static byte[] CreateInvalidCsvBytes_NegativeExecutionTime()
    {
        var csv = @"Date;ExecutionTime;Value
2024-01-15T10:00:00.0000Z;-5.2;150.5";

        return Encoding.UTF8.GetBytes(csv);
    }
    
    public static byte[] CreateInvalidCsvBytes_OnlyHeader()
    {
        var csv = @"Date;ExecutionTime;Value";

        return Encoding.UTF8.GetBytes(csv);
    }
    
    public static byte[] CreateInvalidCsvBytes_MissingColumn()
    {
        var csv = @"Date;ExecutionTime;Value
2024-01-15T10:00:00.0000Z;5.2";

        return Encoding.UTF8.GetBytes(csv);
    }

    public static MultipartFormDataContent CreateMultipartContent(byte[] fileBytes, string fileName)
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
        content.Add(fileContent, "file", fileName);
        return content;
    }
}