using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DotNetify;
using StellaWebInterface.GlobalObjects;

namespace StellaWebInterface.Controllers
{
    public class GeneralVM : MulticastVM
    {
        private readonly IConnectionContext _connectionContext;
        private List<User> _users = new List<User>();
        private Context _context;

        public List<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                _context.Status.ConnectedClients = Users.Count;
                PushUpdates();
            }
        }

        public string Users_itemKey => nameof(User.Id);


        public GeneralVM(IConnectionContext connectionContext)
        {
            _connectionContext = connectionContext;
            _context = Context.Instance;
        }

        public Action<string> AddUser => correlationId =>
        {
            var user = new User(_connectionContext, correlationId);
            lock (Users)
            {
                Users.Add(user);
                this.AddList(nameof(Users), user);
            }

            Console.Out.WriteLine($"Current number of users: {Users.Count}");
        };

        public Action RemoveUser => () =>
        {
            lock (Users)
            {
                var user = Users.FirstOrDefault(x => x.Id == _connectionContext.ConnectionId);
                if (user != null)
                {
                    Users.Remove(user);
                    this.RemoveList(nameof(Users), user.Id);
                }
            }

            Console.Out.WriteLine($"Current number of users: {Users.Count}");
        };

        public override void Dispose()
        {
            RemoveUser();
            Console.Out.WriteLine($"Current number of users: {Users.Count}");
            base.Dispose();
        }
    }
}