using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerApp.DataAccess.Repositories.Interfaces;

namespace ServerApp.DataAccess.Repositories;

public class ProfileImageRepository : IAvatarRepository<ProfileImageEntity>
{
    private readonly MainDbContext _context;

    public ProfileImageRepository(MainDbContext context)
    {
        _context = context;
    }

    public async Task<ImageEntity?> Get(string id, ImageSize size)
    {
        var guidId = Guid.Parse(id);
        var imgs = _context.ProfileImages.Where(x => x.Id == guidId);

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
        var img = await _context.ProfileImages.FirstOrDefaultAsync(x => x.Id == guidId)
            ?? throw new ArgumentException(nameof(id), "Profile image was not found");
        _context.ProfileImages.Remove(img);
    }

    public async Task UpdateOrAddAvatar(ProfileImageEntity avatar)
    {
        if (await _context.ProfileImages.AnyAsync(e => e.Id == avatar.Id))
            _context.ProfileImages.Update(avatar);
        else
            _context.ProfileImages.Add(avatar);

        await _context.SaveChangesAsync();
    }
}