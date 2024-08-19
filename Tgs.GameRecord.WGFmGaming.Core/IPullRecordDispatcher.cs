using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameRecord.WGFmGaming.Core.ApplicationServices;

namespace Tgs.GameRecord.WGFmGaming.Core;
public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
