using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeKeep.Application {
  using Caliburn.Micro;

  using TimeKeep.Application.ViewModels;

  public class AppBootstrapper : Bootstrapper<ShellViewModel> {
    public AppBootstrapper() { }

    protected override void Configure() {
      SetupViewLocator();
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
