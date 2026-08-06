import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from "./ui/sheet";

interface SlideOutPanelProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  description?: string;
  children: React.ReactNode;
}

export default function SlideOutPanel({ open, onOpenChange, title, description, children }: SlideOutPanelProps) {
  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="w-[440px] sm:max-w-[500px] overflow-y-auto px-6 py-5">
        <SheetHeader>
          <SheetTitle className="font-serif text-lg">{title}</SheetTitle>
          {description && <SheetDescription>{description}</SheetDescription>}
        </SheetHeader>
        <div className="flex-1">{children}</div>
      </SheetContent>
    </Sheet>
  );
}
