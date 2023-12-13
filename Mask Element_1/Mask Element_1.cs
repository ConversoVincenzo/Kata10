namespace Mask_Element_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
    using System.Linq;
    using System.Text;
	using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;


	public class Script
	{
		private InteractiveController app;

		/// <summary>
		/// The Script entry point.
		/// IEngine.ShowUI();.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			try
			{
				app = new InteractiveController(engine);

				engine.SetFlag(RunTimeFlags.NoKeyCaching);
				engine.Timeout = TimeSpan.FromHours(10);

				RunSafe(engine);
			}
			catch (ScriptAbortException)
			{
				throw;
			}
			catch (ScriptForceAbortException)
			{
				throw;
			}
			catch (ScriptTimeoutException)
			{
				throw;
			}
			catch (InteractiveUserDetachedException)
			{
				throw;
			}
			catch (Exception e)
			{
				engine.Log("Run|Something went wrong: " + e);
				ShowExceptionDialog(engine, e);
			}
		}

		private void RunSafe(IEngine engine)
		{
			// TODO: Define dialogs here
			MaskElementDialog dialog = new MaskElementDialog(engine);
			app.Run(dialog);
		}

		private void ShowExceptionDialog(IEngine engine, Exception exception)
		{
			ExceptionDialog exceptionDialog = new ExceptionDialog(engine, exception);
			exceptionDialog.OkButton.Pressed += (sender, args) => engine.ExitFail("Something went wrong.");
			if (app.IsRunning) app.ShowDialog(exceptionDialog); else app.Run(exceptionDialog);
		}
	}

	public class MaskElementDialog : Dialog
	{
		private readonly Label helloLabel;
		private readonly Label elementLabel;

		public MaskElementDialog(IEngine engine) : base(engine)
		{
			helloLabel = new Label($"Hi {engine.UserDisplayName} please select element to mask: ");
			elementLabel = new Label("Element");

			var dms = engine.GetDms();
			ElementDropDown = new DropDown(dms.GetElements().Select(x=>x.Name)) { IsDisplayFilterShown = true, IsSorted = true};
			MaskButton = new Button("Mask");

			Title = "Kata 10 Mask Element";

			AddWidget(helloLabel, 0, 0, 1, 2);
			AddWidget(elementLabel, 1, 0);
			AddWidget(ElementDropDown, 1, 1);
			AddWidget(MaskButton, 2, 0, 1, 2);
		}

		public DropDown ElementDropDown { get; private set; }

		public Button MaskButton { get; private set; }
	}

}