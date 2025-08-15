namespace ServerApp.DataAccess.Repositories.Interfaces;

public interface IAvatarRepository<T>
{
    Task<ImageEntity?> Get(string id, ImageSize size);
    Task UpdateOrAddAvatar(T avatar);
    Task Remove(string id);
}