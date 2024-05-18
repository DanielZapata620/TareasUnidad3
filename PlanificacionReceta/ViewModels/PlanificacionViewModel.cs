using PlanificacionReceta.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;


namespace PlanificacionReceta.ViewModels
{
    public class PlanificacionViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RecetaModel> ListaRecetaFiltrada { get; set; } = new();
        List<RecetaModel> RecetasRegistradas = new();

        public string Error { get; set; } = "";
        public string DiaSemana { get; set; } = null!;
        public RecetaModel? Receta { get; set; }

        public ICommand EliminarCommand { get; set; }
        public ICommand MostrarVistaDiaCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }
        public ICommand AgregarCommand { get; set; }

        public ICommand EliminarListaCommand { get; set; }  

        string Ordenamiento = "AZ";

        public string PalabraBusqueda { get; set; } = "";
        public ICommand OrdenarListaCommand { get; set; }

        public ICommand BusquedaCommand { get; set; }

        public ICommand EditarCommand { get; set; }

        public PlanificacionViewModel() 
        {
            MostrarVistaDiaCommand = new Command<string>(MostrarVistaDia);
            CambiarVistaCommand = new Command<string>(MostrarVista);
            OrdenarListaCommand = new Command(Ordenar);
            EliminarCommand = new Command(Eliminar);
            EditarCommand = new Command(Editar);
            AgregarCommand = new Command(Agregar);
            BusquedaCommand = new Command(Busqueda);
            EliminarListaCommand = new Command(EliminarLista);
            Cargar();
        }

        private void EliminarLista()
        {
            RecetasRegistradas.Clear();
            MostrarVista("inicio");
            Guardar();
            
        }

        private void Busqueda()
        {
            if (!string.IsNullOrWhiteSpace(PalabraBusqueda))
            {
                ListaRecetaFiltrada.Clear();
                var datos = RecetasRegistradas.Where(x => x.Dia == DiaSemana && x.Nombre.Contains(PalabraBusqueda));
                foreach (var dato in datos)
                {
                    ListaRecetaFiltrada.Add(dato);
                }
            }
            else
            {
                ActualizarLista();
            }
        }

        private void Ordenar()
        {
            if (Ordenamiento == "AZ") { 
            ListaRecetaFiltrada.Clear();
            var datos = RecetasRegistradas.Where(x => x.Dia == DiaSemana).OrderBy(x => x.Nombre);
            foreach (var dato in datos)
            {
                ListaRecetaFiltrada.Add(dato);
            }
                Ordenamiento = "ZA";
            }
            else if(Ordenamiento == "ZA")
            {
                ListaRecetaFiltrada.Clear();
                var datos = RecetasRegistradas.Where(x => x.Dia == DiaSemana).OrderByDescending(x => x.Nombre);
                foreach (var dato in datos)
                {
                    ListaRecetaFiltrada.Add(dato);
                }
                Ordenamiento = "AZ";
            }

        }

        private void Editar()
        {
            Error = "";
            if (Receta != null)
            {
                if (string.IsNullOrWhiteSpace(Receta.Nombre))
                {
                    Error += "Error: El nombre no es correcto\n";
                }
                if (string.IsNullOrWhiteSpace(Receta.Imagen) || !Receta.Imagen.StartsWith("http") || !Receta.Imagen.EndsWith(".jpg"))
                {
                    Error += "La url de la imagen no es correcta \n ";
                }
                if (string.IsNullOrWhiteSpace(Receta.Instrucciones))
                {
                    Error += "Error:Ingrese unas instrucciones validas\n";
                }
                if (string.IsNullOrWhiteSpace(Receta.Ingredientes))
                {
                    Error += "Error:Los ingredientes no son validos\n";
                }
                if (Receta.TiempoCoccion < 0)
                {
                    Error += "Error:El timepo de coccion no puede ser inferior a 0\n";
                }
                if (Receta.TiempoPreparacion < 0)
                {
                    Error += "Error:El timepo preparcion no puede ser inferior a 0\n";
                }



                PropertyChanged?.Invoke(this, new(nameof(Error)));

                if (Error == "")
                {
                    Receta.Dia = DiaSemana;
                    RecetasRegistradas[IndiceRecetaEditar] = Receta;
                    Guardar();
                    
                    MostrarVistaDia(DiaSemana);
                    PropertyChanged?.Invoke(this, new(nameof(DiaSemana)));
                    Receta = null;

                }
            }
        }

        private void Eliminar(object obj)
        {
            if(Receta != null) 
            { 
            RecetasRegistradas.Remove(Receta);
            Guardar();
            MostrarVistaDia(DiaSemana);
            Receta = null;
            }
        }

        private void Agregar()
        {

            Error = "";
            if(Receta!=null)
            {
                if(string.IsNullOrWhiteSpace(Receta.Nombre))
                {
                    Error += "Error: El nombre no es correcto\n";
                }
                if (string.IsNullOrWhiteSpace(Receta.Imagen) || !Receta.Imagen.StartsWith("http") || !Receta.Imagen.EndsWith(".jpg"))
                {
                    Error += "La url de la imagen no es correcta \n ";
                }
                if (string.IsNullOrWhiteSpace(Receta.Instrucciones))
                {
                    Error += "Error:Ingrese unas instrucciones validas\n";
                }
                if (string.IsNullOrWhiteSpace(Receta.Ingredientes))
                {
                    Error += "Error:Los ingredientes no son validos\n";
                }
                if (Receta.TiempoCoccion < 0)
                {
                    Error += "Error:El timepo de coccion no puede ser inferior a 0\n";
                }
                if (Receta.TiempoPreparacion < 0)
                {
                    Error += "Error:El timepo preparcion no puede ser inferior a 0\n";
                }

                

                PropertyChanged?.Invoke(this, new(nameof(Error)));

                if (Error == "")
                {
                    Receta.Dia = DiaSemana;
                    
                    RecetasRegistradas.Add(Receta);
                    Guardar();
                    ActualizarLista();
                    MostrarVistaDia(DiaSemana);
                    PropertyChanged?.Invoke(this, new(nameof(DiaSemana)));
                    Receta = null;

                }
            }
        }

        void ActualizarLista()
        {
            ListaRecetaFiltrada.Clear();
            var datos = RecetasRegistradas.Where(x => x.Dia == DiaSemana);
            foreach ( var dato in datos )
            {
                ListaRecetaFiltrada.Add(dato);
            }
        }
        int IndiceRecetaEditar;
        private void MostrarVista(string vista)
        {
            
            if (vista == "principal")
            {
                Shell.Current.GoToAsync("//MainPage");
            }
            else if (vista == "agregar")
            {
                Receta = new();
                Error = "";
                Shell.Current.GoToAsync("//Agregar");
                PropertyChanged?.Invoke(this, new(nameof(DiaSemana)));
                PropertyChanged?.Invoke(this, new(nameof(Receta)));
                PropertyChanged?.Invoke(this, new(nameof(Error)));
            }
            else if ( vista == "eliminar")
            {
               if(Receta!=null) 
                { 
                Shell.Current.GoToAsync("//Eliminar");

                PropertyChanged?.Invoke(this, new(nameof(Receta)));
                }
            }
            else if(vista == "editar"&&Receta!=null)
            {
                var clon = new RecetaModel
                {
                    Nombre = Receta.Nombre,
                    Imagen = Receta.Imagen,
                    Ingredientes = Receta.Ingredientes,
                    TiempoCoccion = Receta.TiempoCoccion,
                    TiempoPreparacion = Receta.TiempoPreparacion,
                    Instrucciones = Receta.Instrucciones,
                    Dia = Receta.Dia
                };
                
                IndiceRecetaEditar=RecetasRegistradas.IndexOf(Receta);
                Receta= clon;
                Error = "";
                PropertyChanged?.Invoke(this, new(nameof(Receta)));
                PropertyChanged?.Invoke(this, new(nameof(Error)));
                Shell.Current.GoToAsync("//editar");
                PropertyChanged?.Invoke(this, new(nameof(Error)));
            }
            else if (vista == "detalles" && Receta != null)
            {
                PropertyChanged?.Invoke(this, new(nameof(Receta)));
                
                Shell.Current.GoToAsync("//Detalles");
            }
            else if (vista == "EliminarLista")
            {
                Shell.Current.GoToAsync("//EliminarLista");
            }
            else if (vista == "inicio")
            {
                Shell.Current.GoToAsync("//MainPage");
            }
        }

        private void MostrarVistaDia(string dia)
        {

            Receta = null;
            DiaSemana = dia;
            PropertyChanged?.Invoke(this, new(nameof(DiaSemana)));
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Shell.Current.GoToAsync("//VistaDia");
            ActualizarLista();
            

        }

        string parte2ruta = "/recetas.json";
         void Guardar()
        {
            var ruta = FileSystem.AppDataDirectory;
            File.WriteAllText(ruta + parte2ruta, JsonSerializer.Serialize(RecetasRegistradas));
        }

        void Cargar()
        {
            var ruta=FileSystem.AppDataDirectory;
            if (File.Exists(ruta + parte2ruta))
            {
                var datos = JsonSerializer.Deserialize<List<RecetaModel>>(File.ReadAllText(ruta + parte2ruta));

                if (datos != null)
                {
                    RecetasRegistradas.AddRange(datos);
                    PropertyChanged?.Invoke(this,new(nameof(RecetasRegistradas)));
                }
            }

        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
