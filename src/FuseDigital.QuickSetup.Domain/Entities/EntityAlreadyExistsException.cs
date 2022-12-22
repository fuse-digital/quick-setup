using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace FuseDigital.QuickSetup.Entities;

public class EntityAlreadyExistsException : AbpException
{
    /// <summary>
    /// Type of the entity.
    /// </summary>
    public Type EntityType { get; set; }

    /// <summary>
    /// Id of the Entity.
    /// </summary>
    public object Id { get; set; }

    /// <summary>
    /// Creates a new <see cref="EntityAlreadyExistsException"/> object.
    /// </summary>
    public EntityAlreadyExistsException()
    {

    }

    /// <summary>
    /// Creates a new <see cref="EntityAlreadyExistsException"/> object.
    /// </summary>
    public EntityAlreadyExistsException(Type entityType)
        : this(entityType, null, null)
    {

    }

    /// <summary>
    /// Creates a new <see cref="EntityAlreadyExistsException"/> object.
    /// </summary>
    public EntityAlreadyExistsException(Type entityType, object id)
        : this(entityType, id, null)
    {

    }

    /// <summary>
    /// Creates a new <see cref="EntityAlreadyExistsException"/> object.
    /// </summary>
    public EntityAlreadyExistsException(Type entityType, object id, Exception innerException)
        : base(
            id == null
                ? $"An Entity with the given id already exists. Entity type: {entityType.FullName}"
                : $"An Entity with the given id already exists. Entity type: {entityType.FullName}, id: {id}",
            innerException)
    {
        EntityType = entityType;
        Id = id;
    }

    /// <summary>
    /// Creates a new <see cref="EntityAlreadyExistsException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    public EntityAlreadyExistsException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Creates a new <see cref="EntityAlreadyExistsException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public EntityAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}