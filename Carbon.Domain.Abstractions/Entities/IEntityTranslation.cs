using System.Collections.Generic;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IEntityTranslation
    {
        string LanguageCode { get; set; }
    }
    public interface IEntityTranslation<TEntity, TPrimaryKeyOfMultiLingualEntity> : IEntityTranslation
    {
        string Value { get; set; }
        TEntity Core { get; set; }
        TPrimaryKeyOfMultiLingualEntity CoreId { get; set; }
    }
    public interface IMultiLingualEntity<TTranslation> where TTranslation : class, IEntityTranslation
    {
        ICollection<TTranslation> Translations { get; set; }
    }
    public interface IEntityTranslation<TEntity> : IEntityTranslation<TEntity, long>
    {

    }

}
