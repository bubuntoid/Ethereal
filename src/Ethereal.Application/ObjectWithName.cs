namespace Ethereal.Application
{
    public class ObjectWithName<T>
    {
        public T Object { get; set; }
        
        public string Name { get; set; }

        public ObjectWithName()
        {
                
        }

        public ObjectWithName(T obj, string name)
        {
            Object = obj;
            Name = name;
        }
    }
}