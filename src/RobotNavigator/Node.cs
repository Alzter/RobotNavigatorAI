using System;
using System.Collections.Generic;
using System.Text;

namespace RobotNavigator
{
    public abstract class Node<T>
    {
        protected Node<T> _parent;
        private T _state;
        private int _depth;
        private int _pathCost;
        private string _action;

        public Node(Node<T> parent, T state, int depth, int pathCost, string action){
            _parent = parent;
            _state = state;
            _depth = depth;
            _pathCost = pathCost;
            _action = action;
        }

        public abstract bool IsSuccess();

        public abstract List<Node<T>> Expand(); // Returns a list of the node's children, effectively expanding the node.

        public abstract bool IsStateIdentical(T stateToCompare); // Returns true if the node's state is identical to the state provided.

        public Node<T> Parent
        {
            set
            {
                _parent = value;
            }
            get
            {
                return _parent;
            }
        }

        public T State
        {
            set
            {
                _state = value;
            }
            get
            {
                return _state;
            }
        }

        public int Depth
        {
            set
            {
                _depth = value;
            }
            get
            {
                return _depth;
            }
        }

        public int PathCost
        {
            set
            {
                _pathCost = value;
            }
            get
            {
                return _pathCost;
            }
        }

        public string Action
        {
            get
            {
                if (_action == null) return null;
                return _action.ToLower();
            }
        }

        public abstract float HeuristicValue
        {
            get;
        }
    }
}
