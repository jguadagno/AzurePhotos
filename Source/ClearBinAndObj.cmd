for /d /r . %%d in (bin) do @if exist "%%d" rd /s/q "%%d"
for /d /r . %%d in (obj) do @if exist "%%d" rd /s/q "%%d"