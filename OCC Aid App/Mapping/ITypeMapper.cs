namespace OCC_Aid_App.Mapping
{
    /// <summary>
    /// An interface used for mapping different classes objects. The implementation and scope of classes is the responsibility of every consumer API.
    /// </summary>
    public interface ITypeMapper
    {
        /// <summary>
        /// Map all properties from source object to destination object
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TDestination">The destination type</typeparam>
        /// <param name="sourceObject">The source object</param>
        /// <returns>A new destination object</returns>
        TDestination Map<TSource, TDestination>(TSource sourceObject)
            where TSource : class
            where TDestination : class;
    }
}
