using System.Linq;
using MS.model;

namespace MS.Utils
{
   public class Util
         {
        public MessageString convertMessage(Message message)
        {
            var joinedAttributes = message.Attributes.Aggregate((a, b) => a + ", " + b);

            return new MessageString()
            {
                Key = message.Key,
                Email = message.Email,
                Attributes = joinedAttributes
            };
        }
    }
}