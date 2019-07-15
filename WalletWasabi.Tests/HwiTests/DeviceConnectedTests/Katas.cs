using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WalletWasabi.Hwi2;
using WalletWasabi.Hwi2.Exceptions;
using WalletWasabi.Hwi2.Models;
using Xunit;

namespace WalletWasabi.Tests.HwiTests.DeviceConnectedTests
{
	/// <summary>
	/// Kata tests are intended to be run one by one.
	/// A kata is a type of test that requires user interaction.
	/// User interaction shall be defined in the beginning of the each kata.
	/// Only write katas those require button push responses (eg. don't call setup on trezor.)
	/// </summary>
	public class Katas
	{
		#region SharedVariables

		// Bottleneck: user action on device.
		public TimeSpan ReasonableRequestTimeout { get; } = TimeSpan.FromMinutes(3);

		#endregion SharedVariables

		[Fact]
		public async Task TrezorTKataAsync()
		{
			// --- USER INTERACTIONS ---
			//
			// Connect an already initialized device and unlock it.
			// Run this test.
			//
			// --- USER INTERACTIONS ---

			var client = new HwiClient(Network.Main);
			using (var cts = new CancellationTokenSource(ReasonableRequestTimeout))
			{
				var enumerate = await client.EnumerateAsync(cts.Token);
				HwiEnumerateEntry entry = enumerate.Single();

				string devicePath = entry.Path;
				HardwareWalletVendors deviceType = entry.Type.Value;

				await Assert.ThrowsAsync<HwiException>(async () => await client.SetupAsync(deviceType, devicePath, cts.Token));

				await Assert.ThrowsAsync<HwiException>(async () => await client.RestoreAsync(deviceType, devicePath, cts.Token));

				// Trezor T doesn't support it.
				await Assert.ThrowsAsync<HwiException>(async () => await client.BackupAsync(deviceType, devicePath, cts.Token));

				// Trezor T doesn't support it.
				var ex = await Assert.ThrowsAsync<HwiException>(async () => await client.PromptPinAsync(deviceType, devicePath, cts.Token));

				await Assert.ThrowsAsync<HwiException>(async () => await client.SendPinAsync(deviceType, devicePath, 1111, cts.Token));
				ExtPubKey xpub = await client.GetXpubAsync(deviceType, devicePath, cts.Token);
				Assert.NotNull(xpub);

				// ToDo: displayaddress
				// ToDo: signmessage
				// ToDo: signtx
				// ToDo: --fingerprint
				// ToDo: --password
			}
		}
	}
}
