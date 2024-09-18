using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Xellarium.Shared;

/// <summary>
    /// Обёртка над <see cref="Dictionary{TKey,TValue}"/>, позволяющая использовать значения по умолчанию
    /// При отсутствии элемента в словаре
    /// </summary>
    /// <example>
    /// Можем спокойно использовать нужные операторы для присвоения и т.д.
    /// <code>
    /// var dict = new DefaultDictionary&lt;string, int>(0);
    /// Conole.WriteLine(dict["some"]); // 0, KeyNotFoundException не будет
    /// dict["some"]++;
    /// Console.WriteLine(dict["some"]); // 1
    /// </code>
    /// </example>
    public class DefaultDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ISerializable, IDeserializationCallback
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        
        private readonly Func<TKey, TValue> _defaultValueFactory;
        
        /// <summary>
        /// Словарь, обёрнутый в <see cref="DefaultDictionary{TKey,TValue}"/>
        /// </summary>
        public Dictionary<TKey, TValue> InnerDictionary => _dictionary;

        #region Constructors

        /// <summary>
        /// Создаёт <see cref="DefaultDictionary{TKey,TValue}"/> на основе заданного <see cref="Dictionary{TKey,TValue}"/>
        /// И заданной фабрики для значений по умолчанию
        /// </summary>
        /// <param name="dictionary">Базовый словарь</param>
        /// <param name="defaultValueFactory">Фабрика для значений по умолчанию</param>
        public DefaultDictionary(Dictionary<TKey, TValue> dictionary, Func<TKey, TValue> defaultValueFactory)
        {
            _dictionary = dictionary;
            _defaultValueFactory = defaultValueFactory;
        }
        
        /// <summary>
        /// Создёат <see cref="DefaultDictionary{TKey,TValue}"/>, используя заданную фабрику для значений по умолчанию.
        /// </summary>
        /// <param name="defaultValueFactory">Фабрика для значений по умолчанию</param>
        public DefaultDictionary(Func<TKey, TValue> defaultValueFactory) : this(new Dictionary<TKey, TValue>(), defaultValueFactory)
        {
        }

        /// <summary>
        /// Создаёт <see cref="DefaultDictionary{TKey,TValue}"/>, используя заданное значение по умолчанию
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию</param>
        public DefaultDictionary(TValue defaultValue) : this((_) => defaultValue)
        {
        }
        
        /// <summary>
        /// Создаёт <see cref="DefaultDictionary{TKey,TValue}"/>, используя default как значение по умолчанию
        /// </summary>
        public DefaultDictionary() : this(default(TValue))
        {
            
        }
        
        #endregion
        
        /// <summary>
        /// Возвращает значение из словаря, если не найдено, возвращает значение по умолчанию и добавляет его в словарь
        /// </summary>
        /// <param name="key">Ключ словаря</param>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary.TryGetValue(key, out var value))
                {
                    return value;
                }
                
                _dictionary[key] = _defaultValueFactory(key);
                return _dictionary[key];
            }
            [ExcludeFromCodeCoverage]
            set => _dictionary[key] = value;
        }

        #region DictionaryMethods

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void Add(TKey key, TValue value) => _dictionary.Add(key, value);

        /// <inheritdoc cref="IDictionary{TKey,TValue}.ContainsKey"/>
        [ExcludeFromCodeCoverage]
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public bool Remove(TKey key) => _dictionary.Remove(key);

        /// <inheritdoc cref="IDictionary{TKey,TValue}.TryGetValue"/>
        [ExcludeFromCodeCoverage]
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public IEnumerable<TKey> Keys => _dictionary.Keys;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public IEnumerable<TValue> Values => _dictionary.Values;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void Add(KeyValuePair<TKey, TValue> item) => _dictionary.Add(item.Key, item.Value);
        
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void Clear() => _dictionary.Clear();
        
        /// <inheritdoc cref="ICollection{T}.Count" />
        [ExcludeFromCodeCoverage]
        public int Count => _dictionary.Count;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void GetObjectData(SerializationInfo info, StreamingContext context) => _dictionary.GetObjectData(info, context);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public void OnDeserialization(object sender) => _dictionary.OnDeserialization(sender);
        
        #endregion

        #region ExplicitImplementation

        /// <inheritdoc/>        
        [ExcludeFromCodeCoverage]
        ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;
        
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Contains(item);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Remove(item);
        
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).IsReadOnly;
        
        #endregion
    }