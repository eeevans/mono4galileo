//
// Mono.Messaging
//
// Authors:
//	  Michael Barker (mike@middlesoft.co.uk)
//
//	(C) Ximian, Inc.  http://www.ximian.com
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:(
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Reflection;
using System.Collections;

namespace Mono.Messaging 
{
	/// <summary>
	/// The main entry point for System.Messaging to get a handle on the 
	/// messaging implementation.  It will maintain a single instance of the 
	/// IMessagingProvider (i.e. a singleton) that will be shared between
	/// threads, therefore any implementation of the IMessagingProvider must
	/// be thread safe.
	/// </summary>
	public class MessagingProviderLocator 
	{
		public static readonly TimeSpan InfiniteTimeout = TimeSpan.MaxValue;
		
		private static MessagingProviderLocator instance = new MessagingProviderLocator();		
		private readonly object syncObj = new object();
		private IMessagingProvider provider = null;
		
		private MessagingProviderLocator () {
			string providerName = System.Environment.GetEnvironmentVariable("MONO_MESSAGING_PROVIDER");
			if (providerName == null || providerName == "")
				throw new Exception("Environment Variable MONO_MESSAGING_PROVIDER not defined");
			provider = CreateProvider (providerName);
		}
		
		public static MessagingProviderLocator Instance { get { return instance; } }
		
		public static IMessagingProvider GetProvider ()
		{
			return Instance.provider;
		}
		
		private IMessagingProvider CreateProvider (string className)
		{
			Type t = Type.GetType (className);
			if (t == null)
				throw new Exception ("Can't find class: " + className);
			
			ConstructorInfo ci = t.GetConstructor (BindingFlags.Public | 
			                                       BindingFlags.Instance,
			                                       Type.DefaultBinder,
			                                       new Type[0],
			                                       new ParameterModifier[0]);
			if (ci == null)
				throw new Exception ("Can't find constructor");
			
			return (IMessagingProvider) ci.Invoke (new object[0]);
		}
		
	}
}
