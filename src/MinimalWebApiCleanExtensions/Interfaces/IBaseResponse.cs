namespace MinimalWebApiCleanExtensions.Interfaces;

public interface IBaseResponse<in DtoType, out ResponseType> where ResponseType : IBaseResponse<DtoType, ResponseType>
{
    static abstract ResponseType? ToResponseModel(DtoType? dto);
}
