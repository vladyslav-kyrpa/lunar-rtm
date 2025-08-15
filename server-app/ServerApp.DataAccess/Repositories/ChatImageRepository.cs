
using Microsoft.EntityFrameworkCore;
using ServerApp.DataAccess.Repositories.Interfaces;

namespace ServerApp.DataAccess.Repositories;

public class ChatImageRepository: IAvatarRepository<ChatImageEntity>
{
    private readonly MainDbContext _context;

    public ChatImageRepository(MainDbContext context)
    {
        _context = context;
    }

    public async Task<ImageEntity?> Get(string id, ImageSize size)
    {
        var guidId = Guid.Parse(id);
        var imgs = _context.ChatImages.Where(x => x.Id == guidId);

        if (size == ImageSize.Small)
            return await imgs
                .Select(e => new ImageEntity { Id = e.Id, Bytes = e.BytesSmall })
                .FirstOrDefaultAsync();
        if (size == ImageSize.Medium)
            return await imgs
                .Select(e => new ImageEntity { Id = e.Id, Bytes = e.BytesMedium })
                .FirstOrDefaultAsync();
        if (size == ImageSize.Large)
            return await imgs
                .Select(e => new ImageEntity { Id = e.Id, Bytes = e.BytesLarge })
                .FirstOrDefaultAsync();

        throw new ArgumentException(nameof(size), "Unsupported size");
    }

    public async Task Remove(string id)
    {
        var guidId = Guid.Parse(id);
        var img = await _context.ChatImages.FirstOrDefaultAsync(x => x.Id == guidId)
            ?? throw new ArgumentException(nameof(id), "Profile image was not found");
        _context.ChatImages.Remove(img);
    }

    public async Task UpdateOrAddAvatar(ChatImageEntity avatar)
    {
        if (await _context.ProfileImages.AnyAsync(e => e.Id == avatar.Id))
            _context.ChatImages.Add(avatar);
        else
            _context.ChatImages.Update(avatar);

        await _context.SaveChangesAsync();
    }
}