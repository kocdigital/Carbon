using System.Collections.Generic;

namespace Carbon.Domain.Abstractions.Entities
{
    /// <summary>
    ///     An interface for entities with specific language information.
    /// </summary>
    public interface IEntityTranslation
    {
        string LanguageCode { get; set; }
    }
    /// <summary>
    ///     An interface for wrapping entities with specific language information.
    /// </summary>
    public interface IEntityTranslation<TEntity, TPrimaryKeyOfMultiLingualEntity> : IEntityTranslation
    {
        string Value { get; set; }
        TEntity Core { get; set; }
        TPrimaryKeyOfMultiLingualEntity CoreId { get; set; }
    }

    /// <summary>
    ///     An interface for entities containing information with multiple language options.
    /// </summary>
    public interface IMultiLingualEntity<TTranslation> where TTranslation : class, IEntityTranslation
    {
        ICollection<TTranslation> Translations { get; set; }
    }

    /// <summary>
    ///     An interface for entities with language information and <code>long</code> primary keys.
    /// </summary>
    public interface IEntityTranslation<TEntity> : IEntityTranslation<TEntity, long>
    {

    }

}
