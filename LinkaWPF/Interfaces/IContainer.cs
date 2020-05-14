using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LinkaWPF.Interfaces
{
    public interface IContainer
    {
        void AddElement(UIElement element);
        void RemoveElement(UIElement element);
        double Width { get; }
        double Height { get; }
    }
}
