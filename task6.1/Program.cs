using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

public interface IPhone
{
    string Model { get; set; }
    void Call(string number);
    void SendMessage(string number, string message);
}

// Абстрактный класс
public abstract class MobilePhone : IPhone
{
    public string Model { get; set; }
    public int BatteryLevel { get; set; }
    public bool IsOn { get; set; }

    public MobilePhone(string model)
    {
        Model = model;
        BatteryLevel = 100;
        IsOn = false;
    }

    public virtual void Call(string number)
    {
        Console.WriteLine($"Calling {number} from {Model}");
    }

    public virtual void SendMessage(string number, string message)
    {
        Console.WriteLine($"Sending message to {number} from {Model}: {message}");
    }

    public void TurnOn()
    {
        IsOn = true;
        Console.WriteLine($"{Model} is turned on");
    }

    public void TurnOff()
    {
        IsOn = false;
        Console.WriteLine($"{Model} is turned off");
    }

    public void RechargeBattery()
    {
        BatteryLevel = 100;
        Console.WriteLine($"{Model} battery is fully recharged");
    }
}

public class Smartphone : MobilePhone
{
    public string OperatingSystem { get; set; }
    public int StorageCapacity { get; set; }

    public Smartphone(string model, string operatingSystem, int storageCapacity) : base(model)
    {
        OperatingSystem = operatingSystem;
        StorageCapacity = storageCapacity;
    }

    public override void Call(string number)
    {
        if (IsOn)
        {
            Console.WriteLine($"Calling {number} from {Model} (Smartphone)");
        }
        else
        {
            Console.WriteLine($"{Model} is turned off. Cannot make a call.");
        }
    }

    public override void SendMessage(string number, string message)
    {
        if (IsOn)
        {
            Console.WriteLine($"Sending message to {number} from {Model} (Smartphone): {message}");
        }
        else
        {
            Console.WriteLine($"{Model} is turned off. Cannot send a message.");
        }
    }

    public void InstallApp(string appName)
    {
        Console.WriteLine($"Installing {appName} app on {Model}");
    }

    public void TakePhoto()
    {
        if (IsOn)
        {
            Console.WriteLine($"Taking a photo with {Model} (Smartphone)");
        }
        else
        {
            Console.WriteLine($"{Model} is turned off. Cannot take a photo.");
        }
    }
}

public class FeaturePhone : MobilePhone
{
    public int MemorySize { get; set; }

    public FeaturePhone(string model, int memorySize) : base(model)
    {
        MemorySize = memorySize;
    }

    public override void Call(string number)
    {
        if (IsOn)
        {
            Console.WriteLine($"Calling {number} from {Model} (FeaturePhone)");
        }
        else
        {
            Console.WriteLine($"{Model} is turned off. Cannot make a call.");
        }
    }

    public override void SendMessage(string number, string message)
    {
        if (IsOn)
        {
            Console.WriteLine($"Sending message to {number} from {Model} (FeaturePhone): {message}");
        }
        else
        {
            Console.WriteLine($"{Model} is turned off. Cannot send a message.");
        }
    }

    public void PlaySnakeGame()
    {
        if (IsOn)
        {
            Console.WriteLine($"Playing Snake game on {Model} (FeaturePhone)");
        }
        else
        {
            Console.WriteLine($"{Model} is turned off. Cannot play a game.");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        string pathToAssembly = "task6.1.dll";

        Assembly assembly = Assembly.LoadFrom(pathToAssembly);

        List<Type> phoneTypes = new List<Type>();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.GetInterfaces().Contains(typeof(IPhone)))
            {
                phoneTypes.Add(type);
            }
        }

        List<string> classNames = new List<string>();

        foreach (Type type in phoneTypes)
        {
            classNames.Add(type.Name);
        }

        Form form = new Form();
        ComboBox classDropdown = new ComboBox();
        classDropdown.Items.AddRange(classNames.ToArray());
        classDropdown.SelectedIndexChanged += (sender, e) =>
        {
            string selectedClassName = classDropdown.SelectedItem.ToString();

            Type selectedType = phoneTypes.FirstOrDefault(t => t.Name == selectedClassName);
            MethodInfo[] methods = selectedType.GetMethods();

            foreach (MethodInfo method in methods)
            {
                if (method.DeclaringType == selectedType)
                {
                    TextBox inputTextBox = new TextBox();
                    inputTextBox.Name = method.Name;
                    inputTextBox.Location = new Point(10, 40 + form.Controls.Count * 30);

                    form.Controls.Add(inputTextBox);
                }
            }
        };

        form.Controls.Add(classDropdown);

        Button executeButton = new Button();
        executeButton.Text = "Выполнить";
        executeButton.Location = new Point(10, 40 + form.Controls.Count * 30);
        executeButton.Click += (sender, e) =>
        {
            string selectedClassName = classDropdown.SelectedItem.ToString();
            Type selectedType = phoneTypes.FirstOrDefault(t => t.Name == selectedClassName);

            object instance = Activator.CreateInstance(selectedType);

            foreach (Control control in form.Controls)
            {
                if (control is TextBox inputTextBox)
                {
                    string methodName = inputTextBox.Name;
                    MethodInfo method = selectedType.GetMethod(methodName);

                    object[] parameters = new object[] { inputTextBox.Text };

                    method.Invoke(instance, parameters);
                }
            }
        };

        form.Controls.Add(executeButton);

        Application.Run(form);
    }
}
