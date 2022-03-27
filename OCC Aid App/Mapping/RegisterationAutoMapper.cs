using AutoMapper;
using OCC_Aid_App.Models;

namespace OCC_Aid_App.Mapping
{
    public class RegisterationAutoMapper : ITypeMapper
    {
        #region Constructor
        public RegisterationAutoMapper()
        {
            CreateMappingConfiguration();
        }
        #endregion

        #region Instance Variables
        private static IMapper Mapper { get; set; }
        #endregion

        #region Implementation
        public TDestination Map<TSource, TDestination>(TSource sourceObject)
            where TSource : class
            where TDestination : class
        {
            return (TDestination)Mapper.Map(sourceObject, typeof(TSource), typeof(TDestination));
        }

        private void CreateMappingConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<V1_ZoneRequest, V1_Zone>().ReverseMap();
                cfg.CreateMap<V1_Block, V1_BlockResponse>().ReverseMap();
                cfg.CreateMap<V1_Zone, V1_ZoneResponse>().ReverseMap();
                cfg.CreateMap<V1_ZoneBlock, V1_ZoneBlockResponse>().ReverseMap();

            });
            Mapper = config.CreateMapper();
        }
        #endregion
    }
}
