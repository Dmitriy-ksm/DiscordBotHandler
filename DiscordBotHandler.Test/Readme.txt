You need to provide App.config for the test to pass:
  
  <Target Name="CopyCustomContent" AfterTargets="AfterBuild">
    <Copy SourceFiles="D:\DiscordBotHandler\DiscordBotHandler\App.config" DestinationFiles="$(OutDir)\testhost.dll.config" />
    <Copy SourceFiles="D:\DiscordBotHandler\DiscordBotHandler\App.config" DestinationFiles="$(OutDir)\testhost.x86.dll.config" />
  </Target>
  
  replace SourceFiles="%%%%%%%" with your path to App.config