public class ImageEntity
{
    public Guid Id { get; set; }
    public byte[] BytesSm { get; set; } = [];
    public byte[] BytesMid { get; set; } = [];
    public byte[] BytesLg { get; set; } = [];
}