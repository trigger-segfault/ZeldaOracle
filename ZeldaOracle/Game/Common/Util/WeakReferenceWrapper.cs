using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Util {
	public interface IReadOnlyWeakReference<out T> {
		T Value { get; }
	}
	
	public class WeakReferenceWrapper<T> : IReadOnlyWeakReference<T>
		where T : class {
		private WeakReference<T> reference;
		public WeakReferenceWrapper(T reference) {
			this.reference = new WeakReference<T>(reference);
		}
		public WeakReferenceWrapper(WeakReference<T> reference) {
			this.reference = reference;
		}

		public T Value {
			get {
				T output;
				if (reference.TryGetTarget(out output))
					return output;
				else
					return default(T);
			}
		}

		public bool HasValue {
			get {
				T output;
				return reference.TryGetTarget(out output);
			}
		}
	}
}
