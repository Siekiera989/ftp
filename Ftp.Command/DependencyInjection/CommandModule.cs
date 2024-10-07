using Autofac;
using Ftp.Command.Abstract;

namespace Ftp.Command.DependencyInjection;
public class CommandModule : Module
{
    protected override void Load(ContainerBuilder builder) 
    {
        builder.RegisterType<CwdCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<DeleCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<ListCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<NlstCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<MkdCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<NoOpCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<PassCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<PasvCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<PortCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<PwdCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<QuitCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RetrCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RmdaCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RmdCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RnfrCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<RntoCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<SizeCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<StorCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<TypeCommand>().As<FtpCommandBase>().SingleInstance();
        builder.RegisterType<UserCommand>().As<FtpCommandBase>().SingleInstance();
    }
}
