namespace Domain.Entities;

public class Response<T>
{
    public bool Success { get; private set; }
    public T Data { get; private set; }
    public string ErrorMessage { get; private set; }

    public Response(T data)
    {
        Success = true;
        Data = data;
    }

    public Response(T data, string errorMessage)
    {
        Success = false;
        Data = data;
        ErrorMessage = errorMessage;
    }
}