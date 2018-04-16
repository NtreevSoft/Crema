#pragma once

namespace CremaCode { namespace reader
{
    enum ReadFlag
    {	
        ReadFlag_none = 0,
        ReadFlag_lazy_loading = 1,
        ReadFlag_case_sensitive = 2,

        ReadFlag_mask = 0xff,
    };

	enum DataLocation
	{
		DataLocation_both,
		DataLocation_server,
		DataLocation_client,
	};
} /*namespace CremaCode*/ } /*namespace reader*/

