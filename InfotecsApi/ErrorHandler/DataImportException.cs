namespace InfotecsApi.ErrorHandler;

public class DataImportException: Exception
{
    public DataImportException(string message) : base(message) { }
    
    public DataImportException(string message, Exception innerException) 
        : base(message, innerException) { }
}