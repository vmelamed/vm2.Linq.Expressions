namespace vm2.Linq.Expressions.Serialization.Extensions;

/// <summary>
/// Interface for a reader-writer synchronization mechanism.
/// This interface provides access to a <see cref="ReaderWriterLockSlim"/> and indicates whether the lock is currently
/// held.
/// </summary>
public interface IReaderWriterSync : IDisposable
{
    /// <summary>
    /// Gets the lock.
    /// </summary>
    ReaderWriterLockSlim Lock { get; }

    /// <summary>
    /// Gets a value indicating whether the lock is held and the lock owner can read from the protected resource(s).
    /// </summary>
    bool IsLockHeld { get; }
}

/// <summary>
/// Extension methods for <see cref="ReaderWriterLockSlim"/> that return disposable scope objects for better management
/// of the lifetime of the lock scope via the <c>using</c> statement.
/// </summary>
public static class ReaderWriterLockExtensions
{
    /// <summary>
    /// Gets an upgradeable reader sync. Mere call to <c>new UpgradeableReaderSync(readerWriterLock)</c>
    /// but shows nicely in intellisense.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    /// <returns><see cref="UpgradeableReaderSync" /> object.</returns>
    public static UpgradeableReaderSync UpgradeableReaderLock(this ReaderWriterLockSlim readerWriterLock, int waitMs = 0)
        => new(readerWriterLock, waitMs);

    /// <summary>
    /// Gets a reader sync. Mere call to <c>new ReaderSync(readerWriterLock)</c> but shows nicely in intellisense.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    /// <returns><see cref="ReaderSync" /> object.</returns>
    public static ReaderSync ReaderLock(this ReaderWriterLockSlim readerWriterLock, int waitMs = 0)
        => new(readerWriterLock, waitMs);

    /// <summary>
    /// Gets a writer sync. Mere call to <c>new WriterSync(readerWriterLock)</c> but shows nicely in intellisense.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    /// <returns><see cref="WriterSync" /> object.</returns>
    public static WriterSync WriterLock(this ReaderWriterLockSlim readerWriterLock, int waitMs = 0)
        => new(readerWriterLock, waitMs);
}

/// <summary>
/// With the help of this class create a synchronized multiple readers scope by leveraging the <c>using</c> statement.
/// When the object is created, it attempts to acquire the lock in reader mode. When disposed, it
/// releases the lock if it has previously acquired.
/// </summary>
public sealed class ReaderSync : IReaderWriterSync
{
    /// <inheritdoc/>
    public ReaderWriterLockSlim Lock { get; init; }

    /// <inheritdoc/>
    public bool IsLockHeld { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderSync"/> class with the specified <paramref name="readerWriterLock"/>
    /// and waits the specified number of milliseconds or indefinitely until it acquires the lock in reader mode.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    public ReaderSync(
        ReaderWriterLockSlim readerWriterLock,
        int waitMs = 0)
    {
        Lock = readerWriterLock;
        if (waitMs is 0)
        {
            Lock.EnterReadLock();
            IsLockHeld = true;
        }
        else
            IsLockHeld = Lock.TryEnterReadLock(waitMs);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsLockHeld)
        {
            IsLockHeld = false;
            Lock.ExitReadLock();
        }
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// With the help of this class create a synchronized single writer scope by leveraging the <c>using</c> statement.
/// When the object is created, it attempts to acquire the lock in writer mode. When disposed, it releases the lock if
/// it was previously acquired.
/// </summary>
public sealed class WriterSync : IReaderWriterSync
{
    /// <inheritdoc/>
    public ReaderWriterLockSlim Lock { get; init; }

    /// <inheritdoc/>
    public bool IsLockHeld { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriterSync"/> class with the specified <paramref name="readerWriterLock"/>
    /// and waits the specified number of milliseconds or indefinitely until it acquires the lock in writer mode.
    /// </summary>
    /// <param name="readerWriterLock">The reader-writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    public WriterSync(
        ReaderWriterLockSlim readerWriterLock,
        int waitMs = 0)
    {
        Lock = readerWriterLock;
        if (waitMs is 0)
        {
            Lock.EnterWriteLock();
            IsLockHeld = true;
        }
        else
            IsLockHeld = Lock.TryEnterWriteLock(waitMs);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsLockHeld)
        {
            IsLockHeld = false;
            Lock.ExitWriteLock();
        }
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// With the help of this class create a synchronized reader upgradeable to writer scope by utilizing the <c>using</c> statement.
/// When the object is created, it attempts to acquire the lock in upgradeable reader mode. When disposed, it
/// releases the lock if it has previously acquired.
/// </summary>
public sealed class UpgradeableReaderSync : IReaderWriterSync
{
    /// <inheritdoc/>
    public ReaderWriterLockSlim Lock { get; init; }

    /// <inheritdoc/>
    public bool IsLockHeld { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpgradeableReaderSync" /> class with the specified
    /// <paramref name="readerWriterLock" /> and waits the specified number of milliseconds or indefinitely until it
    /// acquires the lock in upgradeable reader mode.
    /// </summary>
    /// <param name="readerWriterLock">The reader writer lock.</param>
    /// <param name="waitMs">How long to wait for the lock to be acquired in ms. If 0 - wait indefinitely.</param>
    public UpgradeableReaderSync(
        ReaderWriterLockSlim readerWriterLock,
        int waitMs = 0)
    {
        Lock = readerWriterLock;
        if (waitMs is 0)
        {
            Lock.EnterUpgradeableReadLock();
            IsLockHeld = true;
        }
        else
            IsLockHeld = Lock.TryEnterUpgradeableReadLock(waitMs);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsLockHeld)
        {
            Lock.ExitUpgradeableReadLock();
            IsLockHeld = false;
        }
        GC.SuppressFinalize(this);
    }
}
