using System;

namespace AggregateSource
{
    /// <summary>
    /// Class for tracking aggregate meta data and its <see cref="IAggregateRootEntity"/>.
    /// </summary>
    public class Aggregate
    {
        /// <summary>
        /// The default partition an aggregate belongs to if none is specified.
        /// </summary>
        public static readonly string DefaultPartition = "Default";

        readonly string _partition;
        readonly string _identifier;
        readonly IAggregateRootEntity _root;
        readonly int _expectedVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate"/> class.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="expectedVersion">The expected aggregate version.</param>
        /// <param name="root">The aggregate root entity.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="identifier"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="root"/> is null.</exception>
        public Aggregate(string identifier, int expectedVersion, IAggregateRootEntity root)
            : this(DefaultPartition, identifier, expectedVersion, root)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate"/> class.
        /// </summary>
        /// <param name="partition">The aggregate partition.</param>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="expectedVersion">The expected aggregate version.</param>
        /// <param name="root">The aggregate root entity.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="partition"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="identifier"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="root"/> is null.</exception>
        public Aggregate(string partition, string identifier, int expectedVersion, IAggregateRootEntity root)
        {
            if (partition == null) 
                throw new ArgumentNullException("partition");
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (root == null)
                throw new ArgumentNullException("root");
            _partition = partition;
            _identifier = identifier;
            _expectedVersion = expectedVersion;
            _root = root;
        }

        /// <summary>
        /// Gets the partition the aggregate belongs to.
        /// </summary>
        /// <value>
        /// The aggregate partition.
        /// </value>
        public string Partition
        {
            get { return _partition; }
        }

        /// <summary>
        /// Gets the aggregate identifier.
        /// </summary>
        /// <value>
        /// The aggregate identifier.
        /// </value>
        public string Identifier
        {
            get { return _identifier; }
        }

        /// <summary>
        /// Gets the aggregate version.
        /// </summary>
        public Int32 ExpectedVersion
        {
            get { return _expectedVersion; }
        }

        /// <summary>
        /// Gets the aggregate root entity.
        /// </summary>
        /// <value>
        /// The aggregate root entity.
        /// </value>
        public IAggregateRootEntity Root
        {
            get { return _root; }
        }

        /// <summary>
        /// Creates a mutable builder with the same contents as this instance.
        /// </summary>
        /// <returns>An <see cref="AggregateBuilder"/>.</returns>
        public AggregateBuilder ToBuilder()
        {
            return new AggregateBuilder(this);
        }
    }
}