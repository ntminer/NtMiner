using System.Windows;
using System.Windows.Interactivity;

namespace NTMiner {
    public static class BehaviorHelper {
        public static void ApplyBehavior<T>(this DependencyObject p_dependencyObject) where T : Behavior, new() {
            if (p_dependencyObject == null) {
                return;
            }

            BehaviorCollection itemBehaviors = Interaction.GetBehaviors(p_dependencyObject);
            foreach (var behavior in itemBehaviors) {
                if (!(behavior is T)) {
                    continue;
                }

                itemBehaviors.Remove(behavior);
            }

            itemBehaviors.Add(new T());
        }
    }
}