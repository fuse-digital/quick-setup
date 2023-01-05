using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace FuseDigital.QuickSetup.Entities;

public class EntityAlreadyExistsException : BusinessException
{
    /// <summary>
    /// Exceptions are logged with the Error level by default. The Log level can be determined by the exception.
    /// </summary>
    public new LogLevel LogLevel { get; set; } = LogLevel.Warning;

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
    private EntityAlreadyExistsException(Type entityType, object id, Exception innerException)
        : base(
            "IDX001",
            id == null
                ? $"An Entity with the given id already exists. Entity type: {entityType.FullName}"
                : $"An Entity with the given id already exists. Entity type: {entityType.FullName}, id: {id}",
            null,
            innerException
        )
    {
        EntityType = entityType;
        Id = id;
    }
}