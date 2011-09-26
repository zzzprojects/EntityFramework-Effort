using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.TypeGeneration
{
    public class TupleTypeFactory
    {
        public static Type Create(params Type[] typeArguments)
        {
            switch (typeArguments.Length)
            {
                case 1:
                    return typeof(Tuple<>).MakeGenericType(typeArguments);
                case 2:
                    return typeof(Tuple<,>).MakeGenericType(typeArguments);
                case 3:
                    return typeof(Tuple<,,>).MakeGenericType(typeArguments);
                case 4:
                    return typeof(Tuple<,,,>).MakeGenericType(typeArguments);
                case 5:
                    return typeof(Tuple<,,,,>).MakeGenericType(typeArguments);
                case 6:
                    return typeof(Tuple<,,,,,>).MakeGenericType(typeArguments);
                case 7:
                    return typeof(Tuple<,,,,,,>).MakeGenericType(typeArguments);
                default:
                    return AnonymousTypeFactory.Create(
                        typeArguments
                        .Select((type, i) => new { 
                            Name = string.Format("Item{0}", i), 
                            Type = type })
                        .ToDictionary(x => x.Name, x => x.Type));
            }

            
        }
    }
}
