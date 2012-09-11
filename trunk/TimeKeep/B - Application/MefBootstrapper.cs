using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeKeep.Application {
  using System.ComponentModel.Composition;
  using System.ComponentModel.Composition.Hosting;

  using Caliburn.Micro;

  using TimeKeep.Application.ViewModels;

  public class MefBootstrapper : Bootstrapper<IShell> {

    private CompositionContainer container;

    protected override void Configure() {
      SetupViewLocator();
      var catalog = new AggregateCatalog();
      catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));
      container = new CompositionContainer(catalog);
      container.ComposeParts();
    }

    protected override object GetInstance(Type serviceType, string key) {
      string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
      var exports = container.GetExportedValues<object>(contract);

      if (exports.Count() > 0)
        return exports.First();

      throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
    }  

    protected override IEnumerable<object> GetAllInstances(Type serviceType)  
    {  
        return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));  
    }  
  
    protected override void BuildUp(object instance)  
    {  
        container.SatisfyImportsOnce(instance);  
    }

    private void SetupViewLocator() {
      var originalTransformName = ViewLocator.TransformName;

      ViewLocator.TransformName = (typeName, context) => {
        var baseValues = originalTransformName(typeName, context);
        return baseValues.Select(value => value.Replace("Application", "Presentation"));
      };
    }
  }
}
