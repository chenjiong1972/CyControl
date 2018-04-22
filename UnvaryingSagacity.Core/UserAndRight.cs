using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UnvaryingSagacity.Core;
 
namespace UnvaryingSagacity
{
    public class UserAndRight
    {
        public enum RightState
        {
            禁止,
            完全,
            /// <summary>
            /// 保留给以后版本使用
            /// </summary>
            读取,
            /// <summary>
            /// 保留给以后版本使用
            /// </summary>
            写入,
            /// <summary>
            /// 保留给以后版本使用
            /// </summary>
            增加,
            /// <summary>
            /// 保留给以后版本使用
            /// </summary>
            修改,
            /// <summary>
            /// 保留给以后版本使用
            /// </summary>
            删除,
        }

        /// <summary>
        /// 用户
        /// </summary>
        public class User
        {
            public delegate UserAndRight.RightState CallBackRightState(int userID,string rightID);

            private RightRuler _rightRuler;
        
            public int ID { get; set; }
            public string Name { get; set; }
            public int Stop { get; set; }
            public Image Logo { get; set; }
            public int IsVisible { get; set; }
            /// <summary>
            /// 超级用户,即拥有满权限 权限="*"
            /// </summary>
            public bool IsSupper { get; set; }

            public User()
            {
                _rightRuler = new RightRuler();
            }

            public UserAndRight.RightState GetRightState(string rightID)
            {
                if (IsSupper)
                    return RightState.完全; 
                if (_rightRuler.Nodes.ContainsKey (rightID ))
                    return _rightRuler.Nodes[rightID].RightState;
                else
                {
                    foreach (RightRuler ruler in _rightRuler.Nodes)
                    {
                        if (rightID.IndexOf(ruler.ID) == 0)
                            return GetRightState(rightID, ruler);
                    }
                    return UserAndRight.RightState.禁止;
                }
            }

            /// <summary>可以在数据库中动态检索权限
            /// ,由调用方实现callback 方法根据委托CallBackRightState的形参
            /// </summary>
            /// <param name="rightID"></param>
            /// <param name="callback"></param>
            /// <returns></returns>
            public UserAndRight.RightState GetRightState(string rightID,CallBackRightState callback)
            {
                if (IsSupper)
                    return RightState.完全; 
                return callback(this.ID, rightID);
            }

            public RightRuler Ruler { get { return _rightRuler; } }

            private UserAndRight.RightState GetRightState(string rightID,RightRuler ruler)
            {
                if(ruler.ID ==rightID )
                    return ruler.RightState;
                else if (ruler.Nodes.ContainsKey(rightID))
                    return ruler.Nodes[rightID].RightState;
                else
                {
                    foreach (RightRuler rulerChild in ruler.Nodes)
                    {
                        if (rightID.IndexOf(rulerChild.ID) == 0)
                            return GetRightState(rightID, rulerChild);
                    }
                    return UserAndRight.RightState.禁止;
                }
            }

        }

        /// <summary>
        /// 用户组
        /// </summary>
        public class UserGroup 
        {
            private RightRuler _rightRuler = new RightRuler();
            private int[] _users = new int[0];
 
            public string Name { get; set; }
            public string Description { get; set; }

            public RightRuler  Ruler
            {
                get { return _rightRuler; }
            }

            public void FillFrom(int [] userID)
            {
                if (_users.Length < userID.Length)
                    Array.Resize<int>(ref _users, userID.Length);
                userID.CopyTo(_users, 0);
            }

            public int[] UserList()
            {
                return _users;
            }

            public void Add(int userID)
            {
                Array.Resize<int>(ref   _users, _users.Length + 1);
                _users[_users.Length - 1] = userID;  
            }

            public void Remove(int userID)
            {
                int pos = -1;
                for (int i = 0; i < _users.Length; i++)
                {
                    if (_users[i] == userID)
                    {
                        pos = i;
                        break;
                    }
                }
                if (pos != -1)
                {
                    int[] temp = new int[_users.Length - 1];
                    if (pos > 0)
                        Array.Copy(_users, 0, temp, 0, pos);
                    if ((pos + 1) < _users.Length)
                        Array.Copy(_users, pos + 1, temp, pos, _users.Length - (pos + 1));
                    Array.Resize<int>(ref _users, _users.Length - 1);
                    temp.CopyTo(_users, 0); 
                }
                
            }
        }

        /// <summary>
        /// 权限描述
        /// </summary>
        public class Right
        {
            private RightNodes _nodes=new RightNodes ();

            public string ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

            public RightNodes Nodes
            {
                get { return _nodes; }
            }

            public void ToRightRuler(RightRuler ruler)
            {
                ruler.ID = this.ID;
                ruler.Name = this.Name;
                ruler.Description = this.Description;
                ruler.Nodes.Clear();
                if (this.Nodes.Count > 0)
                {
                    foreach (Right r in this.Nodes)
                    {
                        ruler.Nodes.Add(r.ID, new RightRuler());
                        RightRuler ru = ruler.Nodes[r.ID];
                        r.ToRightRuler(ru);
                    }
                }
            }

            /// <summary>
            /// 获得所有下级权限定义的ID数组,不包含本级
            /// </summary>
            /// <returns></returns>
            public Core.TreeNodeToArrary<string>[] Lister(bool searchAllChildren)
            {
                Core.TreeNodeToArrary<string>[] s = new Core.TreeNodeToArrary<string>[0];
                Lister(ref s, this, searchAllChildren);
                return s;
            }

            private void Lister(ref Core.TreeNodeToArrary<string>[] list, Right right, bool searchAllChildren)
            {
                foreach (Right r in right.Nodes)
                {
                    Array.Resize<Core.TreeNodeToArrary<string>>(ref list, list.Length + 1);
                    list[list.Length - 1] = new TreeNodeToArrary<string>();
                    list[list.Length - 1].ID = r.ID;
                    list[list.Length - 1].Parent = right.ID;
                    if (searchAllChildren && r.Nodes.Count > 0)
                    {
                        Lister(ref list, r, searchAllChildren);
                    }
                }
            }
            /// <summary>
            /// 查找一个权限,并返回. 前提是下级的ID字符中必须包含上级的ID
            /// </summary>
            /// <param name="id">要找的权限id</param>
            /// <param name="inAllChildren">是否在所有的下级中搜索</param>
            /// <returns></returns>
            public Right FindRight(string rightID, bool inAllChildren)
            {
                if (Nodes.ContainsKey(rightID))
                    return Nodes[rightID];
                else
                {
                    if (inAllChildren)
                    {
                        foreach (Right right in Nodes)
                        {
                            if (rightID.IndexOf(right.ID) == 0)
                                return FindRight(rightID, right);
                        }
                    }
                    return default(Right);
                }
            }


            private Right FindRight(string rightID, Right right)
            {
                if (right.ID == rightID)
                    return right;
                else if (right.Nodes.ContainsKey(rightID))
                    return right.Nodes[rightID];
                else
                {
                    foreach (Right rightChild in right.Nodes)
                    {
                        if (rightID.IndexOf(rightChild.ID) == 0)
                            return FindRight(rightID, rightChild);
                    }
                    return default(Right);
                }
            }
        }

        /// <summary>权限描述集合
        /// 
        /// </summary>
        public class RightNodes : ListDictionary < Right> { }


        /// <summary>权限规则
        /// 
        /// </summary>
        public class RightRuler : Right
        {
            private RightRulerNodes _nodes=new RightRulerNodes ();

            public RightState RightState { get; set; }

            public new RightRulerNodes Nodes
            {
                get { return _nodes; }
            }

            public void SetRuler(RightState state, bool includeAllChilren)
            {
                this.RightState = state;
                if (includeAllChilren)
                {
                    foreach (RightRuler r in Nodes)
                    {
                        r.RightState = state;
                        if (r.Nodes.Count > 0)
                        {
                            SetRuler(r, state, includeAllChilren);
                        }
                    }
                }
            }

            private void SetRuler(RightRuler ruler, RightState state, bool includeAllChilren)
            {
                ruler.RightState = state;
                if (includeAllChilren)
                {
                    foreach (RightRuler r in ruler.Nodes)
                    {
                        if (r.Nodes.Count > 0)
                        {
                            SetRuler(r, state, includeAllChilren);
                        }
                        else
                            r.RightState = state;
                    }
                }
            }

            public new void ToRightRuler(RightRuler ruler)
            {
                ruler.ID = this.ID;
                ruler.Name = this.Name;
                ruler.Description = this.Description;
                ruler.RightState = this.RightState;
                ruler.Nodes.Clear();
                if (this.Nodes.Count > 0)
                {
                    foreach (RightRuler r in this.Nodes)
                    {
                        ruler.Nodes.Add(r.ID, new RightRuler());
                        RightRuler ru = ruler.Nodes[r.ID];
                        r.ToRightRuler(ru);
                    }
                }
            }
            public string GetRightIDByAllow(string split)
            {
                StringBuilder sb=new StringBuilder ();
                foreach (RightRuler r in Nodes)
                {
                    if (r.Nodes.Count > 0)
                    {
                        GetRightIDByAllow(r, sb,  split);
                    }
                    else
                    {
                        if (r.RightState == RightState.完全)
                        {
                            sb.Append(r.ID);
                            sb.Append(split);
                        }
                    }
                }
                return sb.ToString();
            }

            private void GetRightIDByAllow(RightRuler ruler,StringBuilder sb ,string split)
            {
                foreach (RightRuler r in ruler.Nodes)
                {
                    if (r.Nodes.Count > 0)
                    {
                        GetRightIDByAllow(r, sb, split);
                    }
                    else
                    {
                        if (r.RightState == RightState.完全)
                        {
                            sb.Append(r.ID);
                            sb.Append(split);
                        }
                    }
                }
            }
            
            /// <summary>获得所有下级权限定义的ID数组,不包含本级
            /// 
            /// </summary>
            /// <returns></returns>
            public new Core.TreeNodeToArrary<string>[] Lister(bool searchAllChildren)
            {
                Core.TreeNodeToArrary<string>[] s = new Core.TreeNodeToArrary<string>[0];
                Lister(ref s, this, searchAllChildren);
                return s;
            }

            private void Lister(ref Core.TreeNodeToArrary<string>[] list, RightRuler ruler, bool searchAllChildren)
            {
                foreach (RightRuler r in ruler.Nodes)
                {
                    Array.Resize<Core.TreeNodeToArrary<string>>(ref list, list.Length + 1);
                    list[list.Length - 1] = new TreeNodeToArrary<string>();
                    list[list.Length - 1].ID = r.ID;
                    list[list.Length - 1].Parent = ruler.ID;
                    if (searchAllChildren && r.Nodes.Count > 0)
                    {
                        Lister(ref list, r, searchAllChildren);
                    }
                }
            }
            /// <summary>
            /// 查找一个权限,并返回
            /// </summary>
            /// <param name="id">要找的权限id</param>
            /// <param name="inAllChildren">是否在所有的下级中搜索</param>
            /// <returns></returns>
            public new RightRuler FindRight(string rightID, bool inAllChildren)
            {
                if (Nodes.ContainsKey(rightID))
                    return Nodes[rightID];
                else
                {
                    if (inAllChildren)
                    {
                        foreach (RightRuler right in Nodes)
                        {
                            if (rightID.IndexOf(right.ID) == 0)
                                return FindRight(rightID, right);
                        }
                    }
                    return default(RightRuler);
                }
            }

            private RightRuler FindRight(string rightID, RightRuler right)
            {
                if (right.ID == rightID)
                    return right;
                else if (right.Nodes.ContainsKey(rightID))
                    return right.Nodes[rightID];
                else
                {
                    foreach (RightRuler rightChild in right.Nodes)
                    {
                        if (rightID.IndexOf(rightChild.ID) == 0)
                            return FindRight(rightID, rightChild);
                    }
                    return default(RightRuler);
                }
            }
            
        }

        /// <summary>
        /// 权限规则集合
        /// </summary>
        public class RightRulerNodes : ListDictionary<RightRuler> { }

        public class Users : ListDictionary<UserAndRight.User> { }

        public class UserGroups : ListDictionary<UserAndRight.UserGroup> { } 

    }

    /// <summary>
    /// 预置权限的接口
    /// </summary>
    public interface IRightProvider
    {
        void SetDefaultRight(UserAndRight.Right root);
    }

    public interface IRightProvider2
    {
        void SetDefaultRight(UserAndRight.Right root, System.Data.SqlClient.SqlConnection cnn);
    }
}
