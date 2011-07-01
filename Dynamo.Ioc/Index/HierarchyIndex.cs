using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Not finished - just an idea...

// GroupedHierarchyIndex / NestedIndex ?

//namespace Dynamo.Ioc.Index
//{
//    public class HierarchyIndex : IIndex
//    {
//        // Use GroupedIndex ? or make full implementation instead of just wrapping the grouped index ?
//        private IIndex _parent;

//        public HierarchyIndex(IIndex parent)
//        {
//            if (parent == null)
//                throw new ArgumentNullException("parent");

//            _parent = parent;
//        }



//        public void Add(IRegistration registration)
//        {
//            throw new NotImplementedException();
//        }

//        public IRegistration Get(Type type)
//        {
//            throw new NotImplementedException();
//        }

//        public IRegistration Get(Type type, object key)
//        {
//            throw new NotImplementedException();
//        }

//        public bool TryGet(Type type, out IRegistration registration)
//        {
//            throw new NotImplementedException();
//        }

//        public bool TryGet(Type type, object key, out IRegistration registration)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<IRegistration> GetAll(Type type)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<IRegistration> TryGetAll(Type type)
//        {
//            throw new NotImplementedException();
//        }

//        public bool Contains(Type type)
//        {
//            throw new NotImplementedException();
//        }

//        public bool Contains(Type type, object key)
//        {
//            throw new NotImplementedException();
//        }

//        public bool ContainsAny(Type type)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerator<IRegistration> GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}